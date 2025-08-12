# Azure Deployment PowerShell Script
# Run this from Windows/PowerShell to deploy to Azure

param(
    [string]$Environment = "production",
    [string]$UniqueSuffix = ($env:USERNAME + (Get-Date).ToString("MMdd")),
    [string]$Location = "East US",
    [string]$SubscriptionId = ""
)

$ErrorActionPreference = "Stop"

$ResourceGroup = "framboyan-$Environment-rg"
$AppServicePlan = "framboyan-$Environment-plan"
$ApiAppName = "framboyan-api-$UniqueSuffix"
$ClientAppName = "framboyan-client-$UniqueSuffix"

Write-Host "🚀 Deploying FramboyanScheduler to Azure" -ForegroundColor Green
Write-Host "📍 Environment: $Environment" -ForegroundColor Cyan
Write-Host "🏷️  Unique Suffix: $UniqueSuffix" -ForegroundColor Cyan
Write-Host "📦 Resource Group: $ResourceGroup" -ForegroundColor Cyan
Write-Host "🌐 API URL: https://$ApiAppName.azurewebsites.net" -ForegroundColor Yellow
Write-Host "💻 Client URL: https://$ClientAppName.azurewebsites.net" -ForegroundColor Yellow
Write-Host ""

# Check if Azure CLI is installed
try {
    az --version | Out-Null
} catch {
    Write-Error "❌ Azure CLI is not installed. Please install it first from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
}

# Check if logged in to Azure
try {
    $currentAccount = az account show | ConvertFrom-Json
    Write-Host "📋 Current Azure subscription: $($currentAccount.name)" -ForegroundColor Green
} catch {
    Write-Host "🔐 Please log in to Azure first..." -ForegroundColor Yellow
    az login
    $currentAccount = az account show | ConvertFrom-Json
}

$confirm = Read-Host "Continue with subscription '$($currentAccount.name)'? (y/N)"
if ($confirm -ne "y" -and $confirm -ne "Y") {
    Write-Host "❌ Deployment cancelled" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "🏗️  Creating Azure resources..." -ForegroundColor Green

# Create resource group
Write-Host "📦 Creating resource group..." -ForegroundColor Cyan
az group create --name $ResourceGroup --location $Location --output none

# Create App Service Plan
Write-Host "⚙️  Creating App Service Plan..." -ForegroundColor Cyan
az appservice plan create `
  --name $AppServicePlan `
  --resource-group $ResourceGroup `
  --sku S1 `
  --is-linux `
  --output none

# Create API App Service
Write-Host "🔧 Creating API App Service..." -ForegroundColor Cyan
az webapp create `
  --resource-group $ResourceGroup `
  --plan $AppServicePlan `
  --name $ApiAppName `
  --runtime "DOTNET:9.0" `
  --output none

# Create Client App Service
Write-Host "💻 Creating Client App Service..." -ForegroundColor Cyan
az webapp create `
  --resource-group $ResourceGroup `
  --plan $AppServicePlan `
  --name $ClientAppName `
  --runtime "DOTNET:9.0" `
  --output none

# Generate JWT secret key
$JwtSecret = [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(48))

# Configure API App Settings
Write-Host "⚙️  Configuring API App Settings..." -ForegroundColor Cyan
az webapp config appsettings set `
  --resource-group $ResourceGroup `
  --name $ApiAppName `
  --settings `
    ASPNETCORE_ENVIRONMENT="Production" `
    "JwtSettings__SecretKey=$JwtSecret" `
    "JwtSettings__Issuer=FramboyanScheduler" `
    "JwtSettings__Audience=FramboyanSchedulerUsers" `
    "SecuritySettings__AllowedOrigins=https://$ClientAppName.azurewebsites.net" `
    "ApplicationSettings__ClientUrl=https://$ClientAppName.azurewebsites.net" `
    "ApplicationSettings__ApiUrl=https://$ApiAppName.azurewebsites.net" `
    "ConnectionStrings__DefaultConnection=Data Source=/home/data/framboyan.db" `
  --output none

# Configure Client App Settings
Write-Host "🌐 Configuring Client App Settings..." -ForegroundColor Cyan
az webapp config appsettings set `
  --resource-group $ResourceGroup `
  --name $ClientAppName `
  --settings `
    ASPNETCORE_ENVIRONMENT="Production" `
    "API_BASE_URL=https://$ApiAppName.azurewebsites.net" `
  --output none

# Enable logging
Write-Host "📊 Enabling application logging..." -ForegroundColor Cyan
az webapp log config `
  --resource-group $ResourceGroup `
  --name $ApiAppName `
  --application-logging filesystem `
  --level information `
  --output none

az webapp log config `
  --resource-group $ResourceGroup `
  --name $ClientAppName `
  --application-logging filesystem `
  --level information `
  --output none

# Build applications
Write-Host "📦 Building applications..." -ForegroundColor Green

$DeployDir = "./azure-deploy-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
New-Item -ItemType Directory -Path $DeployDir -Force | Out-Null

# Build API
Write-Host "🔧 Building API..." -ForegroundColor Cyan
Set-Location "src/FramboyanSchedulerApi"
dotnet publish -c Release -o "../../$DeployDir/api"
Set-Location "../.."

# Build Client
Write-Host "💻 Building Client..." -ForegroundColor Cyan
Set-Location "src/Client"
dotnet publish -c Release -o "../../$DeployDir/client"
Set-Location "../.."

# Update client config
Write-Host "⚙️  Updating client configuration..." -ForegroundColor Cyan
$configPath = "$DeployDir/client/wwwroot/config.js"
if (Test-Path $configPath) {
    (Get-Content $configPath) -replace "#{API_BASE_URL}#", "https://$ApiAppName.azurewebsites.net" | Set-Content $configPath
}

# Deploy API
Write-Host "🚀 Deploying API..." -ForegroundColor Green
Set-Location "$DeployDir/api"
Compress-Archive -Path * -DestinationPath "../api-deploy.zip" -Force
Set-Location "../.."

az webapp deployment source config-zip `
  --resource-group $ResourceGroup `
  --name $ApiAppName `
  --src "$DeployDir/api-deploy.zip" `
  --output none

# Deploy Client
Write-Host "🌐 Deploying Client..." -ForegroundColor Green
Set-Location "$DeployDir/client"
Compress-Archive -Path * -DestinationPath "../client-deploy.zip" -Force
Set-Location "../.."

az webapp deployment source config-zip `
  --resource-group $ResourceGroup `
  --name $ClientAppName `
  --src "$DeployDir/client-deploy.zip" `
  --output none

# Clean up
Write-Host "🧹 Cleaning up..." -ForegroundColor Cyan
Remove-Item -Recurse -Force $DeployDir

# Test deployments
Write-Host "🔍 Testing deployments..." -ForegroundColor Cyan
Start-Sleep 30

try {
    $ApiStatus = (Invoke-WebRequest -Uri "https://$ApiAppName.azurewebsites.net/health" -UseBasicParsing -ErrorAction SilentlyContinue).StatusCode
} catch {
    $ApiStatus = "Error"
}

try {
    $ClientStatus = (Invoke-WebRequest -Uri "https://$ClientAppName.azurewebsites.net" -UseBasicParsing -ErrorAction SilentlyContinue).StatusCode
} catch {
    $ClientStatus = "Error"
}

Write-Host ""
Write-Host "✅ Deployment completed!" -ForegroundColor Green
Write-Host ""
Write-Host "📊 Deployment Summary:" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host "🌐 API URL:        https://$ApiAppName.azurewebsites.net" -ForegroundColor Cyan
Write-Host "💻 Client URL:     https://$ClientAppName.azurewebsites.net" -ForegroundColor Cyan
Write-Host "🔧 API Status:     $ApiStatus" -ForegroundColor $(if($ApiStatus -eq 200){"Green"}else{"Yellow"})
Write-Host "💻 Client Status:  $ClientStatus" -ForegroundColor $(if($ClientStatus -eq 200){"Green"}else{"Yellow"})
Write-Host "📦 Resource Group: $ResourceGroup" -ForegroundColor Cyan
Write-Host "🔑 JWT Secret:     [Generated and configured]" -ForegroundColor Green
Write-Host ""

if ($ApiStatus -eq 200) {
    Write-Host "✅ API is responding correctly" -ForegroundColor Green
} else {
    Write-Host "⚠️  API may need a few more minutes to start" -ForegroundColor Yellow
}

if ($ClientStatus -eq 200) {
    Write-Host "✅ Client is responding correctly" -ForegroundColor Green
} else {
    Write-Host "⚠️  Client may need a few more minutes to start" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "📋 Next Steps:" -ForegroundColor Yellow
Write-Host "1. Visit your client URL to test the application"
Write-Host "2. Configure SMTP settings in Azure portal"
Write-Host "3. Set up custom domain (optional)"
Write-Host "4. Configure SSL certificate for custom domain"
Write-Host "5. Set up monitoring and alerts"
Write-Host ""
Write-Host "💰 Estimated monthly cost: ~`$78-85 (S1 App Service Plan)" -ForegroundColor Green
Write-Host ""
Write-Host "🎉 Your FramboyanScheduler is now live on Azure!" -ForegroundColor Green
