# Framboyan Dance Studio - Mobile App 📱

A Flutter mobile application for the Framboyan Dance Studio scheduling system.

## 🏃‍♀️ Features

- **User Authentication** - Login/Register with JWT tokens
- **Class Browsing** - View available dance classes in calendar format
- **Class Booking** - Book and cancel class reservations
- **Check-in System** - Check into classes you've booked
- **Profile Management** - View bookings and membership status
- **Real-time Updates** - Live class availability and capacity

## 🛠️ Tech Stack

- **Framework:** Flutter (Dart)
- **Backend API:** ASP.NET Core (.NET 9)
- **Authentication:** JWT Bearer tokens
- **State Management:** Provider/Riverpod
- **HTTP Client:** Dio
- **Local Storage:** SharedPreferences
- **Push Notifications:** Firebase Cloud Messaging

## 📱 Supported Platforms

- ✅ Android (API 21+)
- ✅ iOS (iOS 12+)
- 🔮 Future: Web version

## 🚀 Getting Started

### Prerequisites

- Flutter SDK (3.0+)
- Android Studio / Xcode
- VS Code with Flutter extension
- Git

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/SirMalelo/FramboyanSchedulerMobileApp.git
   cd FramboyanSchedulerMobileApp
   ```

2. **Install dependencies:**
   ```bash
   flutter pub get
   ```

3. **Configure API endpoint:**
   ```dart
   // lib/config/api_config.dart
   const String API_BASE_URL = 'http://your-api-domain.com';
   ```

4. **Run the app:**
   ```bash
   flutter run
   ```

## 🔗 Backend Integration

This mobile app connects to the **FramboyanSchedulerApp** backend API:
- Repository: https://github.com/SirMalelo/FramboyanSchedulerApp
- API Documentation: See `docs/API_INTEGRATION.md`

### Key API Endpoints Used:
- `POST /api/auth/login` - User authentication
- `POST /api/auth/register` - User registration
- `GET /api/class/calendar` - Fetch available classes
- `POST /api/class/book/{id}` - Book a class
- `POST /api/class/checkin/{id}` - Check into a class
- `GET /api/class/my-bookings` - Get user's bookings

## 📁 Project Structure

```
lib/
├── main.dart                 # App entry point
├── config/                   # Configuration files
│   ├── api_config.dart
│   └── theme_config.dart
├── models/                   # Data models
│   ├── user.dart
│   ├── class.dart
│   └── booking.dart
├── services/                 # Business logic & API calls
│   ├── auth_service.dart
│   ├── class_service.dart
│   └── api_client.dart
├── providers/                # State management
│   ├── auth_provider.dart
│   └── class_provider.dart
├── screens/                  # UI screens
│   ├── auth/
│   │   ├── login_screen.dart
│   │   └── register_screen.dart
│   ├── classes/
│   │   ├── classes_screen.dart
│   │   └── class_detail_screen.dart
│   └── profile/
│       └── profile_screen.dart
├── widgets/                  # Reusable UI components
│   ├── class_card.dart
│   ├── loading_widget.dart
│   └── custom_button.dart
└── utils/                    # Helper functions
    ├── constants.dart
    └── validators.dart
```

## 🎨 Design System

The mobile app follows the studio's branding from the web application:
- **Primary Color:** Dynamic (from API site customization)
- **Typography:** Custom fonts matching web app
- **Components:** Material Design 3 with custom styling

## 🔐 Security

- JWT token secure storage
- API request/response encryption
- Biometric authentication (planned)
- Session timeout handling

## 📊 Development Status

- [ ] **Phase 1:** Authentication & Basic UI
- [ ] **Phase 2:** Class Browsing & Booking
- [ ] **Phase 3:** Check-in System
- [ ] **Phase 4:** Push Notifications
- [ ] **Phase 5:** Offline Support

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit changes: `git commit -m 'Add amazing feature'`
4. Push to branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

For support, email support@framboyandancestudio.com or create an issue in this repository.

---

**Built with ❤️ for the Framboyan Dance Studio community**
