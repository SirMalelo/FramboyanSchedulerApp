# FramboyanScheduler Deployment Guide 🚀

## Pre-Deployment Checklist

### ✅ Critical Security Items
- [ ] **Replace JWT Secret Key** - Generate 32+ character random key
- [ ] **Configure SMTP Settings** - Set up real email service
- [ ] **Set up SSL Certificate** - Use Let's Encrypt or commercial cert
- [ ] **Update CORS Origins** - Restrict to your domain only
- [ ] **Database Security** - Secure file permissions and backup strategy

### ✅ Configuration Items
- [ ] **Environment Variables** - Set all production values
- [ ] **Connection Strings** - Point to production database
- [ ] **Client Base URL** - Update to production domain
- [ ] **API Base URL** - Update in client configuration
- [ ] **Logging Configuration** - Set appropriate log levels

### ✅ Infrastructure Requirements
- [ ] **Linux Server** - Ubuntu 20.04+ or similar
- [ ] **.NET 9 Runtime** - Install on server
- [ ] **Nginx** - Web server for reverse proxy
- [ ] **Database Storage** - Secure location for SQLite file
- [ ] **Backup Strategy** - Automated backups configured

## Quick Deployment Steps

### 1. Prepare Production Configuration

```bash
# Run the setup script on your server
sudo bash deployment/production-setup.sh

# Edit the environment file with your values
sudo nano /etc/framboyan/environment
```

### 2. Update Application Settings

**Update API Program.cs for production:**
```csharp
// Replace hardcoded values with configuration
var jwtKey = builder.Configuration["JwtSettings:SecretKey"] ?? 
             throw new InvalidOperationException("JWT key not configured");
```

**Update Client base URL:**
```csharp
// In Client/Program.cs
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://api.yourdomain.com") }
);
```

### 3. Build and Deploy

```bash
# Build API
cd src/FramboyanSchedulerApi
dotnet publish -c Release -o /var/www/framboyan-api

# Build Client
cd ../Client
dotnet publish -c Release -o /var/www/framboyan-client/temp
cp -r /var/www/framboyan-client/temp/wwwroot/* /var/www/framboyan-client/
rm -rf /var/www/framboyan-client/temp
```

### 4. Database Migration

```bash
# Run migrations on production
cd /var/www/framboyan-api
dotnet FramboyanSchedulerApi.dll --migrate-database
```

### 5. Start Services

```bash
# Enable and start the API service
sudo systemctl enable --now framboyan-api

# Enable nginx site
sudo ln -s /etc/nginx/sites-available/framboyan /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

## Security Hardening

### 1. Generate Secure JWT Key
```bash
# Generate a secure random key
openssl rand -base64 48
```

### 2. Set Up SSL with Let's Encrypt
```bash
# Install certbot
sudo apt install certbot python3-certbot-nginx

# Get certificate
sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com
```

### 3. Configure Firewall
```bash
# Set up UFW firewall
sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable
```

## Monitoring and Maintenance

### 1. Log Monitoring
```bash
# API logs
sudo journalctl -u framboyan-api -f

# Nginx logs
sudo tail -f /var/log/nginx/access.log
sudo tail -f /var/log/nginx/error.log
```

### 2. Health Checks
```bash
# Check API health
curl https://api.yourdomain.com/api/health

# Check service status
sudo systemctl status framboyan-api
```

### 3. Database Backups
```bash
# Manual backup
sudo /usr/local/bin/backup-framboyan.sh

# Automated backups (add to crontab)
crontab -e
# Add: 0 2 * * * /usr/local/bin/backup-framboyan.sh
```

## Performance Optimization

### 1. Nginx Compression
Add to nginx config:
```nginx
gzip on;
gzip_vary on;
gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
```

### 2. Database Optimization
```sql
-- Regular maintenance
PRAGMA optimize;
VACUUM;
REINDEX;
```

### 3. Static File Caching
```nginx
location ~* \.(css|js|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
    expires 1y;
    add_header Cache-Control "public, immutable";
}
```

## Troubleshooting

### Common Issues

1. **Service won't start**
   - Check logs: `sudo journalctl -u framboyan-api`
   - Verify environment file: `/etc/framboyan/environment`
   - Check file permissions

2. **Database errors**
   - Verify database path exists and is writable
   - Check SQLite file permissions
   - Ensure migrations are applied

3. **Client can't connect to API**
   - Verify CORS settings
   - Check nginx configuration
   - Confirm SSL certificate

### Emergency Recovery
```bash
# Restore from backup
sudo systemctl stop framboyan-api
cd /var/lib/framboyan
sudo tar -xzf /var/backups/framboyan/framboyan_backup_YYYYMMDD_HHMMSS.tar.gz
sudo systemctl start framboyan-api
```

## Production Checklist

Before going live:

- [ ] All environment variables set
- [ ] SSL certificate installed and working
- [ ] Database backed up
- [ ] Nginx configuration tested
- [ ] Firewall configured
- [ ] Monitoring set up
- [ ] Backup automation working
- [ ] Health checks responding
- [ ] SMTP email tested
- [ ] Admin account created
- [ ] Test user registration and login flow

## Support

For deployment issues:
1. Check application logs
2. Verify configuration files
3. Test network connectivity
4. Review nginx error logs
5. Check system resources (disk, memory, CPU)
