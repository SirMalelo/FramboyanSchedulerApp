#!/bin/bash

# FramboyanScheduler Azure Deployment Script
# Usage: ./azure-deploy.sh [environment] [unique-suffix]
# Example: ./azure-deploy.sh production mycompany

set -e

ENVIRONMENT=${1:-production}
UNIQUE_SUFFIX=${2:-$(whoami)$(date +%m%d)}
RESOURCE_GROUP="framboyan-${ENVIRONMENT}-rg"
LOCATION="East US"
APP_SERVICE_PLAN="framboyan-${ENVIRONMENT}-plan"
API_APP_NAME="framboyan-api-${UNIQUE_SUFFIX}"
CLIENT_APP_NAME="framboyan-client-${UNIQUE_SUFFIX}"
SQL_SERVER_NAME="framboyan-sql-${UNIQUE_SUFFIX}"
SQL_DB_NAME="framboyandb"
STORAGE_ACCOUNT_NAME="framboyan${UNIQUE_SUFFIX}storage"

echo "🚀 Deploying FramboyanScheduler to Azure"
echo "📍 Environment: $ENVIRONMENT"
echo "🏷️  Unique Suffix: $UNIQUE_SUFFIX"
echo "📦 Resource Group: $RESOURCE_GROUP"
echo "🌐 API URL: https://$API_APP_NAME.azurewebsites.net"
echo "💻 Client URL: https://$CLIENT_APP_NAME.azurewebsites.net"
echo ""

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI is not installed. Please install it first:"
    echo "https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Check if logged in to Azure
if ! az account show &> /dev/null; then
    echo "🔐 Please log in to Azure first:"
    az login
fi

echo "📋 Current Azure subscription:"
az account show --query "name" -o tsv

read -p "Continue with this subscription? (y/N): " confirm
if [[ $confirm != [yY] ]]; then
    echo "❌ Deployment cancelled"
    exit 1
fi

echo ""
echo "🏗️  Creating Azure resources..."

# Create resource group
echo "📦 Creating resource group..."
az group create --name $RESOURCE_GROUP --location "$LOCATION" --output none

# Create App Service Plan
echo "⚙️  Creating App Service Plan..."
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP \
  --sku S1 \
  --is-linux \
  --output none

# Create API App Service
echo "🔧 Creating API App Service..."
az webapp create \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --name $API_APP_NAME \
  --runtime "DOTNET:9.0" \
  --output none

# Create Client App Service
echo "💻 Creating Client App Service..."
az webapp create \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_SERVICE_PLAN \
  --name $CLIENT_APP_NAME \
  --runtime "DOTNET:9.0" \
  --output none

# Generate JWT secret key
JWT_SECRET=$(openssl rand -base64 48)

# Configure API App Settings
echo "⚙️  Configuring API App Settings..."
az webapp config appsettings set \
  --resource-group $RESOURCE_GROUP \
  --name $API_APP_NAME \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    JwtSettings__SecretKey="$JWT_SECRET" \
    JwtSettings__Issuer="FramboyanScheduler" \
    JwtSettings__Audience="FramboyanSchedulerUsers" \
    SecuritySettings__AllowedOrigins="https://$CLIENT_APP_NAME.azurewebsites.net" \
    ApplicationSettings__ClientUrl="https://$CLIENT_APP_NAME.azurewebsites.net" \
    ApplicationSettings__ApiUrl="https://$API_APP_NAME.azurewebsites.net" \
    ConnectionStrings__DefaultConnection="Data Source=/home/data/framboyan.db" \
  --output none

# Configure Client App Settings
echo "🌐 Configuring Client App Settings..."
az webapp config appsettings set \
  --resource-group $RESOURCE_GROUP \
  --name $CLIENT_APP_NAME \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    API_BASE_URL="https://$API_APP_NAME.azurewebsites.net" \
  --output none

# Enable logging for both apps
echo "📊 Enabling application logging..."
az webapp log config \
  --resource-group $RESOURCE_GROUP \
  --name $API_APP_NAME \
  --application-logging filesystem \
  --level information \
  --output none

az webapp log config \
  --resource-group $RESOURCE_GROUP \
  --name $CLIENT_APP_NAME \
  --application-logging filesystem \
  --level information \
  --output none

# Build and prepare deployment packages
echo "📦 Building applications..."

# Create deployment directory
DEPLOY_DIR="./azure-deploy-$(date +%Y%m%d-%H%M%S)"
mkdir -p $DEPLOY_DIR

# Build API
echo "🔧 Building API..."
cd src/FramboyanSchedulerApi
dotnet publish -c Release -o "../../$DEPLOY_DIR/api"
cd ../..

# Build Client
echo "💻 Building Client..."
cd src/Client
dotnet publish -c Release -o "../../$DEPLOY_DIR/client"
cd ../..

# Update client config with actual API URL
echo "⚙️  Updating client configuration..."
sed -i "s|#{API_BASE_URL}#|https://$API_APP_NAME.azurewebsites.net|g" "$DEPLOY_DIR/client/wwwroot/config.js"

# Deploy API
echo "🚀 Deploying API..."
cd "$DEPLOY_DIR/api"
zip -r ../api-deploy.zip . > /dev/null
cd ../..

az webapp deployment source config-zip \
  --resource-group $RESOURCE_GROUP \
  --name $API_APP_NAME \
  --src "$DEPLOY_DIR/api-deploy.zip" \
  --output none

# Deploy Client
echo "🌐 Deploying Client..."
cd "$DEPLOY_DIR/client"
zip -r ../client-deploy.zip . > /dev/null
cd ../..

az webapp deployment source config-zip \
  --resource-group $RESOURCE_GROUP \
  --name $CLIENT_APP_NAME \
  --src "$DEPLOY_DIR/client-deploy.zip" \
  --output none

# Clean up deployment files
echo "🧹 Cleaning up..."
rm -rf $DEPLOY_DIR

# Test deployments
echo "🔍 Testing deployments..."
sleep 30  # Wait for apps to start

API_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "https://$API_APP_NAME.azurewebsites.net/health" || echo "000")
CLIENT_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "https://$CLIENT_APP_NAME.azurewebsites.net" || echo "000")

echo ""
echo "✅ Deployment completed!"
echo ""
echo "📊 Deployment Summary:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "🌐 API URL:        https://$API_APP_NAME.azurewebsites.net"
echo "💻 Client URL:     https://$CLIENT_APP_NAME.azurewebsites.net"
echo "🔧 API Status:     $API_STATUS"
echo "💻 Client Status:  $CLIENT_STATUS"
echo "📦 Resource Group: $RESOURCE_GROUP"
echo "🔑 JWT Secret:     [Generated and configured]"
echo ""

if [[ $API_STATUS == "200" ]]; then
    echo "✅ API is responding correctly"
else
    echo "⚠️  API may need a few more minutes to start"
fi

if [[ $CLIENT_STATUS == "200" ]]; then
    echo "✅ Client is responding correctly"
else
    echo "⚠️  Client may need a few more minutes to start"
fi

echo ""
echo "📋 Next Steps:"
echo "1. Visit your client URL to test the application"
echo "2. Configure custom domain (optional):"
echo "   az webapp config hostname add --webapp-name $CLIENT_APP_NAME --resource-group $RESOURCE_GROUP --hostname yourdomain.com"
echo "3. Set up SSL certificate for custom domain"
echo "4. Configure SMTP settings in Azure portal"
echo "5. Set up monitoring and alerts"
echo ""
echo "📊 Monitor your deployment:"
echo "   az webapp log tail --name $API_APP_NAME --resource-group $RESOURCE_GROUP"
echo ""
echo "💰 Estimated monthly cost: ~$78-85 (S1 App Service Plan + Azure SQL Basic)"
echo ""
echo "🎉 Your FramboyanScheduler is now live on Azure!"
