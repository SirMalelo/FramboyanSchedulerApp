#!/bin/bash

# FramboyanScheduler Production Environment Setup Script
# Run this script on your production server

set -e

echo "🚀 Setting up FramboyanScheduler for Production..."

# Create application directory
sudo mkdir -p /var/lib/framboyan
sudo mkdir -p /var/log/framboyan
sudo mkdir -p /etc/framboyan

# Set permissions
sudo chown -R www-data:www-data /var/lib/framboyan
sudo chown -R www-data:www-data /var/log/framboyan
sudo chmod 750 /var/lib/framboyan
sudo chmod 750 /var/log/framboyan

echo "📁 Directories created successfully"

# Environment variables template
cat > /tmp/framboyan.env << EOF
# JWT Configuration
JWT_SECRET_KEY=YOUR_SUPER_SECRET_JWT_KEY_HERE_MINIMUM_32_CHARACTERS

# Database Configuration
DATABASE_PATH=/var/lib/framboyan/framboyan.db

# Email Configuration (SMTP)
SMTP_HOST=your-smtp-server.com
SMTP_PORT=587
SMTP_USER=your-email@domain.com
SMTP_PASS=your-email-password
SMTP_FROM=noreply@yourdomain.com

# Application URLs
CLIENT_URL=https://yourdomain.com
API_URL=https://api.yourdomain.com
ALLOWED_ORIGINS=https://yourdomain.com,https://www.yourdomain.com

# SSL Certificate (if using Let's Encrypt)
CERT_PATH=/etc/letsencrypt/live/yourdomain.com/fullchain.pem
CERT_KEY_PATH=/etc/letsencrypt/live/yourdomain.com/privkey.pem

# Backup Configuration
BACKUP_RETENTION_DAYS=30
BACKUP_LOCATION=/var/backups/framboyan
EOF

sudo mv /tmp/framboyan.env /etc/framboyan/environment
sudo chown root:root /etc/framboyan/environment
sudo chmod 600 /etc/framboyan/environment

echo "🔐 Environment template created at /etc/framboyan/environment"
echo "⚠️  IMPORTANT: Edit /etc/framboyan/environment with your actual values!"

# Create systemd service
cat > /tmp/framboyan-api.service << EOF
[Unit]
Description=FramboyanScheduler API
After=network.target

[Service]
Type=notify
Restart=always
RestartSec=5
User=www-data
Group=www-data
WorkingDirectory=/var/www/framboyan-api
ExecStart=/usr/bin/dotnet FramboyanSchedulerApi.dll
Environment=ASPNETCORE_ENVIRONMENT=Production
EnvironmentFile=/etc/framboyan/environment
SyslogIdentifier=framboyan-api
TimeoutStopSec=20
KillMode=process

# Security settings
NoNewPrivileges=true
PrivateTmp=true
ProtectSystem=strict
ReadWritePaths=/var/lib/framboyan
ReadWritePaths=/var/log/framboyan

[Install]
WantedBy=multi-user.target
EOF

sudo mv /tmp/framboyan-api.service /etc/systemd/system/
sudo systemctl daemon-reload

echo "⚙️  Systemd service created"

# Create backup script
cat > /tmp/backup-framboyan.sh << 'EOF'
#!/bin/bash

# FramboyanScheduler Backup Script
# Add to crontab: 0 2 * * * /usr/local/bin/backup-framboyan.sh

source /etc/framboyan/environment

TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="framboyan_backup_${TIMESTAMP}.tar.gz"

# Create backup directory
mkdir -p $BACKUP_LOCATION

# Stop the service
systemctl stop framboyan-api

# Create backup
cd /var/lib/framboyan
tar -czf "$BACKUP_LOCATION/$BACKUP_FILE" .

# Start the service
systemctl start framboyan-api

# Remove old backups
find $BACKUP_LOCATION -name "framboyan_backup_*.tar.gz" -mtime +$BACKUP_RETENTION_DAYS -delete

echo "Backup completed: $BACKUP_FILE"
EOF

sudo mv /tmp/backup-framboyan.sh /usr/local/bin/
sudo chmod +x /usr/local/bin/backup-framboyan.sh

echo "💾 Backup script created at /usr/local/bin/backup-framboyan.sh"

# Nginx configuration template
cat > /tmp/framboyan-nginx.conf << 'EOF'
# FramboyanScheduler Nginx Configuration

upstream framboyan_api {
    server 127.0.0.1:5117;
}

server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name yourdomain.com www.yourdomain.com;

    ssl_certificate /etc/letsencrypt/live/yourdomain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/yourdomain.com/privkey.pem;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-RSA-AES256-GCM-SHA512:DHE-RSA-AES256-GCM-SHA512:ECDHE-RSA-AES256-GCM-SHA384:DHE-RSA-AES256-GCM-SHA384;
    ssl_prefer_server_ciphers off;

    # API endpoints
    location /api/ {
        proxy_pass http://framboyan_api;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        proxy_buffering off;
    }

    # Static files (Blazor app)
    location / {
        root /var/www/framboyan-client;
        try_files $uri $uri/ /index.html;
        
        # Cache static assets
        location ~* \.(css|js|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
            expires 1y;
            add_header Cache-Control "public, immutable";
        }
    }

    # Security headers
    add_header X-Frame-Options DENY;
    add_header X-Content-Type-Options nosniff;
    add_header X-XSS-Protection "1; mode=block";
    add_header Strict-Transport-Security "max-age=63072000; includeSubDomains; preload";
}
EOF

sudo mv /tmp/framboyan-nginx.conf /etc/nginx/sites-available/framboyan
echo "🌐 Nginx configuration created at /etc/nginx/sites-available/framboyan"

echo ""
echo "✅ Production setup completed!"
echo ""
echo "📋 Next steps:"
echo "1. Edit /etc/framboyan/environment with your actual values"
echo "2. Install SSL certificate (Let's Encrypt recommended)"
echo "3. Deploy your application files to /var/www/framboyan-api and /var/www/framboyan-client"
echo "4. Enable nginx site: sudo ln -s /etc/nginx/sites-available/framboyan /etc/nginx/sites-enabled/"
echo "5. Test nginx config: sudo nginx -t"
echo "6. Reload nginx: sudo systemctl reload nginx"
echo "7. Start the service: sudo systemctl enable --now framboyan-api"
echo "8. Add backup to crontab: crontab -e"
echo "   Add line: 0 2 * * * /usr/local/bin/backup-framboyan.sh"
echo ""
echo "🔍 Monitor logs with: sudo journalctl -u framboyan-api -f"
