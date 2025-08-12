# Azure Deployment Guide for FramboyanScheduler 🚀

## Overview
This guide covers deploying FramboyanScheduler to Microsoft Azure using App Service for both the API and client, with Azure SQL Database or SQLite storage.

## Azure Services Required

### 1. App Service Plan
- **Recommended:** Standard S1 or higher
- **Region:** Choose closest to your users
- **OS:** Linux (recommended) or Windows

### 2. App Services (2 instances)
- **API App Service:** `framboyan-api`
- **Client App Service:** `framboyan-client`

### 3. Database Options
- **Option A:** Azure SQL Database (recommended for production)
- **Option B:** SQLite with Azure Files (simpler, lower cost)

### 4. Additional Services
- **Azure Key Vault:** For secrets management
- **Application Insights:** For monitoring
- **Azure CDN:** For static file delivery (optional)

## Quick Deployment Options

### Option 1: Azure CLI Deployment (Recommended)
```bash
# Run the Azure deployment script
./deployment/azure-deploy.sh
```

### Option 2: Azure Portal Manual Setup
Follow the step-by-step guide below.

### Option 3: GitHub Actions CI/CD
Use the provided workflow file for automated deployments.

## Manual Setup Steps

### 1. Create Resource Group
```bash
az group create --name framboyan-rg --location "East US"
```

### 2. Create App Service Plan
```bash
az appservice plan create \
  --name framboyan-plan \
  --resource-group framboyan-rg \
  --sku S1 \
  --is-linux
```

### 3. Create App Services
```bash
# API App Service
az webapp create \
  --resource-group framboyan-rg \
  --plan framboyan-plan \
  --name framboyan-api-[your-unique-suffix] \
  --runtime "DOTNETCORE:9.0"

# Client App Service  
az webapp create \
  --resource-group framboyan-rg \
  --plan framboyan-plan \
  --name framboyan-client-[your-unique-suffix] \
  --runtime "DOTNETCORE:9.0"
```

### 4. Configure App Settings

#### API App Service Settings
```bash
az webapp config appsettings set \
  --resource-group framboyan-rg \
  --name framboyan-api-[your-unique-suffix] \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    JwtSettings__SecretKey="your-48-character-secret-key" \
    JwtSettings__Issuer="FramboyanScheduler" \
    JwtSettings__Audience="FramboyanSchedulerUsers" \
    SecuritySettings__AllowedOrigins="https://framboyan-client-[your-unique-suffix].azurewebsites.net" \
    ApplicationSettings__ClientUrl="https://framboyan-client-[your-unique-suffix].azurewebsites.net" \
    ApplicationSettings__ApiUrl="https://framboyan-api-[your-unique-suffix].azurewebsites.net"
```

#### Client App Service Settings
```bash
az webapp config appsettings set \
  --resource-group framboyan-rg \
  --name framboyan-client-[your-unique-suffix] \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    API_BASE_URL="https://framboyan-api-[your-unique-suffix].azurewebsites.net"
```

### 5. Database Setup

#### Option A: Azure SQL Database
```bash
# Create SQL Server
az sql server create \
  --name framboyan-sql-server \
  --resource-group framboyan-rg \
  --location "East US" \
  --admin-user framboyanadmin \
  --admin-password "YourSecurePassword123!"

# Create SQL Database
az sql db create \
  --resource-group framboyan-rg \
  --server framboyan-sql-server \
  --name framboyandb \
  --service-objective S0

# Configure connection string
az webapp config connection-string set \
  --resource-group framboyan-rg \
  --name framboyan-api-[your-unique-suffix] \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:framboyan-sql-server.database.windows.net,1433;Initial Catalog=framboyandb;Persist Security Info=False;User ID=framboyanadmin;Password=YourSecurePassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

#### Option B: SQLite with Azure Files (Simpler)
```bash
# Create storage account
az storage account create \
  --name framboyanstorage \
  --resource-group framboyan-rg \
  --location "East US" \
  --sku Standard_LRS

# The SQLite file will be stored in the App Service file system
# No additional configuration needed
```

## Deployment Scripts

### Azure CLI Deployment Script
I'll create a comprehensive deployment script that automates the entire process.

### GitHub Actions Workflow
I'll create a CI/CD pipeline that automatically deploys when you push to main branch.

## Configuration Files

### Production Configuration
The `appsettings.Production.json` will be automatically used in Azure with environment-specific settings.

### Environment Variables
All sensitive data will be stored in Azure App Service Configuration (equivalent to environment variables).

## SSL/HTTPS
- Azure App Service provides free SSL certificates
- Custom domain SSL certificates can be uploaded or managed through Azure

## Monitoring and Logging
- Application Insights integration for performance monitoring
- Azure App Service logs for debugging
- Health check endpoints for availability monitoring

## Scaling and Performance
- Auto-scaling rules based on CPU/memory
- Azure CDN for static content delivery
- Connection pooling for database connections

## Cost Estimation

### Basic Setup (Development/Small Business)
- **App Service Plan S1:** ~$73/month
- **Two App Services:** Included in plan
- **Azure SQL Basic:** ~$5/month (or SQLite for free)
- **Application Insights:** Free tier available
- **Total:** ~$78-85/month

### Production Setup
- **App Service Plan P1V3:** ~$146/month
- **Azure SQL S1:** ~$20/month
- **Application Insights:** ~$5-10/month
- **Total:** ~$171-180/month

## Security Features
- Azure Active Directory integration (optional)
- Azure Key Vault for secrets
- Network security groups
- SSL/TLS encryption
- DDoS protection

## Backup and Disaster Recovery
- Automated App Service backups
- Azure SQL automated backups
- Cross-region deployment options

## Next Steps
1. Run the Azure deployment script
2. Configure custom domain (optional)
3. Set up monitoring alerts
4. Configure automated backups
5. Set up CI/CD pipeline

---

**Ready to deploy to Azure!** The platform provides excellent .NET support and all the enterprise features you need for a production studio management system.
