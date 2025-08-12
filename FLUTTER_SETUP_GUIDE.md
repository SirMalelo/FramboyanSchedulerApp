# 🚀 Flutter Setup Guide for Your Dance Studio App

## Step 1: Install Flutter

### Option A: Using Git (Recommended)
1. Open PowerShell as Administrator
2. Navigate to C:\ drive:
```powershell
cd C:\
```

3. Clone Flutter:
```powershell
git clone https://github.com/flutter/flutter.git -b stable
```

4. Add Flutter to your PATH:
```powershell
$env:PATH = "C:\flutter\bin;$env:PATH"
```

### Option B: Manual Download
1. Go to https://docs.flutter.dev/get-started/install/windows
2. Download Flutter SDK
3. Extract to C:\flutter
4. Add C:\flutter\bin to your Windows PATH

## Step 2: Verify Installation
```powershell
flutter doctor
```

## Step 3: Install Android Studio (for Android development)
1. Download from https://developer.android.com/studio
2. Install with default settings
3. Install Android SDK through Android Studio

## Step 4: Install VS Code Flutter Extension
1. Open VS Code
2. Go to Extensions (Ctrl+Shift+X)
3. Search and install "Flutter" extension by Dart Code

## Step 5: Create Your First App
```powershell
cd C:\Projects
flutter create dance_studio_app
cd dance_studio_app
```

## Step 6: Test Your Setup
```powershell
flutter run
```

## Next Steps After Setup
Once Flutter is installed, I'll help you:
1. Create the dance studio app structure
2. Set up API integration with your existing backend
3. Build the login/register screens
4. Create the class booking interface

---

## 📱 Your App Structure Will Look Like:
```
dance_studio_app/
├── lib/
│   ├── main.dart                 # App entry point
│   ├── models/                   # Data models
│   │   ├── user.dart
│   │   ├── class.dart
│   │   └── booking.dart
│   ├── services/                 # API calls
│   │   ├── auth_service.dart
│   │   ├── class_service.dart
│   │   └── api_client.dart
│   ├── screens/                  # App screens
│   │   ├── login_screen.dart
│   │   ├── classes_screen.dart
│   │   ├── profile_screen.dart
│   │   └── booking_screen.dart
│   └── widgets/                  # Reusable components
│       ├── class_card.dart
│       └── loading_widget.dart
└── pubspec.yaml                  # Dependencies
```

## 🎯 App Features We'll Build:
✅ User Authentication (Login/Register)
✅ Browse Classes (Calendar View)
✅ Book/Cancel Classes
✅ Check-in to Classes
✅ View My Bookings
✅ User Profile
✅ Push Notifications (Advanced)

Let me know when you've completed the Flutter installation!
