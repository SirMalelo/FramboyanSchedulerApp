# Security Configuration Guide 🔒

## Critical Security Items Before Deployment

### 1. JWT Secret Key Configuration ⚠️ **CRITICAL**

**Current Issue:** Using hardcoded fallback key in development
```csharp
// NEVER use this in production!
var key = "super_secret_jwt_key_123!";
```

**Solution:** Generate a secure random key
```bash
# Generate a 256-bit (32-byte) random key
openssl rand -base64 48

# Or use PowerShell on Windows
[System.Web.Security.Membership]::GeneratePassword(48, 10)
```

**Configuration:**
```json
// appsettings.Production.json
{
  "JwtSettings": {
    "SecretKey": "YOUR_GENERATED_SECRET_KEY_HERE",
    "Issuer": "FramboyanScheduler",
    "Audience": "FramboyanSchedulerUsers"
  }
}
```

### 2. Database Security 🗄️

**Current Issue:** SQLite file in application directory
- File permissions too open
- No backup encryption
- Database path exposed

**Solutions:**
```bash
# Secure database location
sudo mkdir -p /var/lib/framboyan
sudo chown www-data:www-data /var/lib/framboyan
sudo chmod 750 /var/lib/framboyan

# Secure file permissions
chmod 600 /var/lib/framboyan/framboyan.db
```

**Connection String:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/var/lib/framboyan/framboyan.db"
  }
}
```

### 3. CORS Configuration 🌐

**Current Issue:** `AllowAnyOrigin()` in production
```csharp
// DANGEROUS for production!
policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
```

**Solution:** Restrict to your domains
```json
{
  "SecuritySettings": {
    "AllowedOrigins": [
      "https://yourdomain.com",
      "https://www.yourdomain.com"
    ]
  }
}
```

### 4. HTTPS and SSL Configuration 🔐

**Requirements:**
- Force HTTPS in production
- Valid SSL certificate
- Security headers

**Implementation:**
```csharp
if (!isDevelopment)
{
    app.UseHsts();
    app.UseHttpsRedirection();
    
    // Security headers
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        await next();
    });
}
```

### 5. Password Security 💪

**Enhanced Requirements:**
```csharp
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = true;
options.Password.RequiredLength = 12; // Increased for production
options.Password.RequiredUniqueChars = 3;

// Account lockout
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.AllowedForNewUsers = true;
```

### 6. Email Security 📧

**Current Issue:** Using `FakeEmailService`

**Solution:** Implement real email service
```csharp
// Register proper email service
if (isDevelopment)
{
    builder.Services.AddScoped<IEmailService, FakeEmailService>();
}
else
{
    builder.Services.AddScoped<IEmailService, SmtpEmailService>();
}
```

**SMTP Configuration:**
```json
{
  "Smtp": {
    "Host": "smtp.yourdomain.com",
    "Port": 587,
    "User": "noreply@yourdomain.com",
    "Pass": "secure_app_password",
    "From": "noreply@yourdomain.com",
    "EnableSsl": true
  }
}
```

## Environment Variables Setup

### Required Environment Variables
```bash
# /etc/framboyan/environment

# JWT Security
JWT_SECRET_KEY=your_super_secure_48_character_random_key_here_abc123

# Database
DATABASE_PATH=/var/lib/framboyan/framboyan.db

# SMTP Email
SMTP_HOST=smtp.yourdomain.com
SMTP_PORT=587
SMTP_USER=noreply@yourdomain.com
SMTP_PASS=your_app_password
SMTP_FROM=noreply@yourdomain.com

# Application URLs
CLIENT_URL=https://yourdomain.com
API_URL=https://api.yourdomain.com
ALLOWED_ORIGINS=https://yourdomain.com,https://www.yourdomain.com

# SSL Certificates
CERT_PATH=/etc/letsencrypt/live/yourdomain.com/fullchain.pem
CERT_KEY_PATH=/etc/letsencrypt/live/yourdomain.com/privkey.pem
```

## Firewall Configuration

### UFW Setup
```bash
# Install UFW
sudo apt install ufw

# Default policies
sudo ufw default deny incoming
sudo ufw default allow outgoing

# Allow SSH (change port if needed)
sudo ufw allow 22/tcp

# Allow HTTP/HTTPS
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Enable firewall
sudo ufw enable

# Check status
sudo ufw status verbose
```

### Fail2Ban Setup
```bash
# Install fail2ban
sudo apt install fail2ban

# Create custom jail for your app
sudo tee /etc/fail2ban/jail.local << EOF
[DEFAULT]
bantime = 3600
findtime = 600
maxretry = 5

[sshd]
enabled = true

[nginx-http-auth]
enabled = true

[nginx-noscript]
enabled = true

[nginx-badbots]
enabled = true
EOF

sudo systemctl enable --now fail2ban
```

## File Permissions

### Secure File Structure
```bash
# Application files
sudo chown -R www-data:www-data /var/www/framboyan-api
sudo chown -R www-data:www-data /var/www/framboyan-client
sudo chmod -R 750 /var/www/framboyan-api
sudo chmod -R 755 /var/www/framboyan-client

# Database directory
sudo chown www-data:www-data /var/lib/framboyan
sudo chmod 750 /var/lib/framboyan
sudo chmod 600 /var/lib/framboyan/framboyan.db

# Log files
sudo chown -R www-data:www-data /var/log/framboyan
sudo chmod 750 /var/log/framboyan

# Configuration files
sudo chown root:www-data /etc/framboyan/environment
sudo chmod 640 /etc/framboyan/environment
```

## Backup Security

### Encrypted Backups
```bash
#!/bin/bash
# Enhanced backup script with encryption

TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="framboyan_backup_${TIMESTAMP}.tar.gz"
ENCRYPTED_FILE="framboyan_backup_${TIMESTAMP}.tar.gz.gpg"

# Create backup
cd /var/lib/framboyan
tar -czf "/tmp/$BACKUP_FILE" .

# Encrypt backup
gpg --cipher-algo AES256 --compress-algo 1 --s2k-mode 3 \
    --s2k-digest-algo SHA512 --s2k-count 65536 --symmetric \
    --output "$BACKUP_LOCATION/$ENCRYPTED_FILE" "/tmp/$BACKUP_FILE"

# Remove unencrypted backup
rm "/tmp/$BACKUP_FILE"

echo "Encrypted backup completed: $ENCRYPTED_FILE"
```

## Monitoring and Alerting

### Log Monitoring
```bash
# Set up log rotation
sudo tee /etc/logrotate.d/framboyan << EOF
/var/log/framboyan/*.log {
    daily
    missingok
    rotate 30
    compress
    delaycompress
    notifempty
    create 644 www-data www-data
    postrotate
        systemctl reload framboyan-api
    endscript
}
EOF
```

### Health Check Monitoring
```bash
#!/bin/bash
# Health check script for monitoring

API_URL="https://api.yourdomain.com/health"
ALERT_EMAIL="admin@yourdomain.com"

if ! curl -f -s "$API_URL" > /dev/null; then
    echo "API health check failed" | mail -s "FramboyanScheduler Alert" "$ALERT_EMAIL"
fi
```

## Security Checklist

Before going live, verify:

- [ ] JWT secret key is 48+ characters and random
- [ ] Database file has proper permissions (600)
- [ ] CORS is restricted to your domains only
- [ ] HTTPS is enforced with valid certificate
- [ ] Password requirements are production-grade
- [ ] Real email service is configured
- [ ] Firewall is properly configured
- [ ] Fail2ban is installed and configured
- [ ] File permissions are secure
- [ ] Backups are encrypted
- [ ] Log rotation is configured
- [ ] Health monitoring is set up
- [ ] Security headers are enabled
- [ ] Swagger is disabled in production
- [ ] Environment variables are properly set

## Regular Security Maintenance

### Weekly Tasks
- Review access logs for suspicious activity
- Check for failed login attempts
- Verify backup integrity
- Update SSL certificates if needed

### Monthly Tasks
- Update system packages
- Review and rotate secrets if needed
- Check database integrity
- Audit user accounts

### Security Updates
- Monitor .NET security advisories
- Update packages regularly
- Review and update security policies
- Conduct security assessments
