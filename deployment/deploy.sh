#!/bin/bash

# FramboyanScheduler Deployment Script
# Usage: ./deploy.sh [environment] [domain]
# Example: ./deploy.sh production yourdomain.com

set -e

ENVIRONMENT=${1:-production}
DOMAIN=${2:-localhost}
API_URL="https://api.$DOMAIN"
CLIENT_URL="https://$DOMAIN"

echo "🚀 Deploying FramboyanScheduler to $ENVIRONMENT"
echo "📍 Domain: $DOMAIN"
echo "🔗 API URL: $API_URL"
echo "🌐 Client URL: $CLIENT_URL"

# Validate environment
if [[ "$ENVIRONMENT" != "production" && "$ENVIRONMENT" != "staging" ]]; then
    echo "❌ Invalid environment. Use 'production' or 'staging'"
    exit 1
fi

# Create build directory
BUILD_DIR="./build-$ENVIRONMENT"
rm -rf $BUILD_DIR
mkdir -p $BUILD_DIR

echo "📦 Building API..."
cd src/FramboyanSchedulerApi
dotnet publish -c Release -o ../../$BUILD_DIR/api
cd ../..

echo "📦 Building Client..."
cd src/Client
dotnet publish -c Release -o ../../$BUILD_DIR/client-temp
cd ../..

# Prepare client files
mkdir -p $BUILD_DIR/client
cp -r $BUILD_DIR/client-temp/wwwroot/* $BUILD_DIR/client/
rm -rf $BUILD_DIR/client-temp

# Replace configuration placeholders
echo "⚙️  Configuring for $ENVIRONMENT..."

# Update client config
sed -i "s|#{API_BASE_URL}#|$API_URL|g" $BUILD_DIR/client/config.js

# Update production appsettings (you'll need to set actual values)
cp src/FramboyanSchedulerApi/appsettings.Production.json $BUILD_DIR/api/
echo "⚠️  Remember to update $BUILD_DIR/api/appsettings.Production.json with actual values!"

# Create deployment package
echo "📁 Creating deployment package..."
cd $BUILD_DIR
tar -czf ../framboyan-$ENVIRONMENT-$(date +%Y%m%d-%H%M%S).tar.gz .
cd ..

echo "✅ Build completed successfully!"
echo ""
echo "📋 Next steps:"
echo "1. Upload the tar.gz file to your server"
echo "2. Extract to /var/www/framboyan-api and /var/www/framboyan-client"
echo "3. Update environment variables in /etc/framboyan/environment"
echo "4. Restart the service: sudo systemctl restart framboyan-api"
echo "5. Test the deployment"
echo ""
echo "🔍 Files are ready in: $BUILD_DIR/"
