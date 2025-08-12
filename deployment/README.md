# FramboyanScheduler - Azure Deployment Guide

## Quick Start
Your application is ready for Azure deployment! Here's everything you need:

### Prerequisites
- Azure CLI installed
- Active Azure subscription
- .NET 9.0 SDK

### One-Click Deploy (Windows PowerShell)
```powershell
.\deployment\azure-deploy.ps1
```

### Custom Deploy
```powershell
.\deployment\azure-deploy.ps1 -Environment "prod" -Location "West US 2" -UniqueSuffix "mycompany"
```

## Cost Estimate
- **Basic Setup**: ~$78-85/month
- **Production Setup**: ~$150-200/month

## Manual Deployment Steps

### 1. Create Azure Resources
```bash
# Login to Azure
az login

# Create resource group
az group create --name framboyan-rg --location "East US"

# Create App Service Plan
az appservice plan create \
  --name framboyan-plan \
  --resource-group framboyan-rg \
  --sku S1 \
  --is-linux

# Create App Services
az webapp create \
  --resource-group framboyan-rg \
  --plan framboyan-plan \
  --name framboyan-api-unique \
  --runtime "DOTNET:9.0"

az webapp create \
  --resource-group framboyan-rg \
  --plan framboyan-plan \
  --name framboyan-client-unique \
  --runtime "DOTNET:9.0"
```

### 2. Configure Environment Variables

#### API Configuration
```bash
az webapp config appsettings set \
  --resource-group framboyan-rg \
  --name framboyan-api-unique \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    JwtSettings__SecretKey="[GENERATE-SECURE-KEY]" \
    JwtSettings__Issuer="FramboyanScheduler" \
    JwtSettings__Audience="FramboyanSchedulerUsers" \
    SecuritySettings__AllowedOrigins="https://framboyan-client-unique.azurewebsites.net" \
    ApplicationSettings__ClientUrl="https://framboyan-client-unique.azurewebsites.net"
```

#### Client Configuration
```bash
az webapp config appsettings set \
  --resource-group framboyan-rg \
  --name framboyan-client-unique \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    API_BASE_URL="https://framboyan-api-unique.azurewebsites.net"
```

### 3. Build and Deploy

#### Build API
```bash
cd src/FramboyanSchedulerApi
dotnet publish -c Release -o ./publish
```

#### Build Client
```bash
cd src/Client
dotnet publish -c Release -o ./publish
```

#### Deploy to Azure
```bash
# Deploy API
az webapp deployment source config-zip \
  --resource-group framboyan-rg \
  --name framboyan-api-unique \
  --src api-package.zip

# Deploy Client
az webapp deployment source config-zip \
  --resource-group framboyan-rg \
  --name framboyan-client-unique \
  --src client-package.zip
```

## Database Configuration

Your SQLite database will be automatically created on first run. For production, consider:

1. **Azure SQL Database** (recommended for scale)
2. **PostgreSQL on Azure** (open source option)
3. **Keep SQLite** (simplest, good for small/medium apps)

### Upgrading to Azure SQL Database
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=FramboyanDB;Persist Security Info=False;User ID=your-username;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

## Security Considerations

### 1. JWT Secret Key
Generate a secure key:
```powershell
$key = [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
```

### 2. CORS Configuration
Ensure your client URL is properly configured in API settings.

### 3. HTTPS Only
Azure App Service enforces HTTPS by default.

## Monitoring & Health Checks

Your API includes health checks at `/health`. Monitor this endpoint for uptime.

### Application Insights (Optional)
Add Application Insights for detailed monitoring:
```bash
az monitor app-insights component create \
  --app framboyan-insights \
  --location "East US" \
  --resource-group framboyan-rg
```

## Custom Domain Setup

### 1. Add Custom Domain
```bash
az webapp config hostname add \
  --webapp-name framboyan-client-unique \
  --resource-group framboyan-rg \
  --hostname yourdomain.com
```

### 2. SSL Certificate
Azure provides free SSL certificates for custom domains.

## Troubleshooting

### Common Issues

1. **App Won't Start**
   - Check logs: `az webapp log tail --name framboyan-api-unique --resource-group framboyan-rg`
   - Verify environment variables are set correctly

2. **CORS Errors**
   - Ensure client URL is in API's AllowedOrigins setting
   - Check for typos in URLs

3. **Authentication Issues**
   - Verify JWT secret key is set and same across deployments
   - Check issuer/audience configuration

### Viewing Logs
```bash
# API logs
az webapp log tail --name framboyan-api-unique --resource-group framboyan-rg

# Client logs  
az webapp log tail --name framboyan-client-unique --resource-group framboyan-rg
```

## Performance Optimization

### 1. App Service Plan Scaling
- **S1**: Good for development/small production
- **P1V3**: Better performance, auto-scaling
- **P2V3**: High performance, premium features

### 2. Database Performance
For high traffic, consider:
- Azure SQL Database with higher DTU
- Connection pooling
- Read replicas

### 3. CDN (Optional)
For global users, add Azure CDN:
```bash
az cdn profile create \
  --name framboyan-cdn \
  --resource-group framboyan-rg \
  --sku Standard_Microsoft
```

## Backup Strategy

### 1. App Service Backups
```bash
az webapp config backup create \
  --resource-group framboyan-rg \
  --webapp-name framboyan-api-unique \
  --backup-name daily-backup \
  --storage-account-url "[STORAGE-URL]"
```

### 2. Database Backups
- SQLite: Backup the database file
- Azure SQL: Automatic backups included

## Environment Variables Reference

### API Required Settings
- `ASPNETCORE_ENVIRONMENT`: "Production"
- `JwtSettings__SecretKey`: Secure random key
- `JwtSettings__Issuer`: "FramboyanScheduler"
- `JwtSettings__Audience`: "FramboyanSchedulerUsers"
- `SecuritySettings__AllowedOrigins`: Client URL

### Client Required Settings
- `ASPNETCORE_ENVIRONMENT`: "Production"
- `API_BASE_URL`: API URL

### Optional Settings
- `ApplicationSettings__SmtpServer`: Email server
- `ApplicationSettings__SmtpPort`: Email port
- `ApplicationSettings__SmtpUsername`: Email username
- `ApplicationSettings__SmtpPassword`: Email password

## Support & Maintenance

### Regular Tasks
1. Monitor app performance via Azure portal
2. Review security recommendations
3. Update dependencies regularly
4. Monitor costs and optimize as needed

### Scaling Considerations
- **Vertical Scaling**: Upgrade App Service Plan tier
- **Horizontal Scaling**: Enable auto-scaling rules
- **Database Scaling**: Upgrade to Azure SQL for better performance

---

## Quick Links
- [Azure Portal](https://portal.azure.com)
- [App Service Documentation](https://docs.microsoft.com/en-us/azure/app-service/)
- [Azure CLI Reference](https://docs.microsoft.com/en-us/cli/azure/)

Your FramboyanScheduler is now ready for professional Azure deployment! 🚀
