# API Integration Guide 🔗

This document explains how the mobile app integrates with the FramboyanSchedulerApp backend API.

## 🌐 Base Configuration

### API Base URL
```dart
// Production
const String API_BASE_URL = 'https://your-production-domain.com';

// Development
const String API_BASE_URL = 'http://localhost:5117';
```

### Authentication Headers
```dart
final headers = {
  'Content-Type': 'application/json',
  'Authorization': 'Bearer $jwtToken', // For authenticated requests
};
```

## 🔐 Authentication Endpoints

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "user-id",
    "email": "user@example.com",
    "fullName": "John Doe",
    "roles": ["Student"]
  }
}
```

### Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "newuser@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!"
}
```

## 📅 Class Management Endpoints

### Get Classes (Calendar)
```http
GET /api/class/calendar?startDate=2025-08-01&endDate=2025-08-31
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Salsa Beginners",
    "description": "Perfect for first-time dancers",
    "startTime": "2025-08-11T19:00:00",
    "endTime": "2025-08-11T20:00:00",
    "maxCapacity": 20,
    "instructorName": "Maria Rodriguez",
    "bookedCount": 15,
    "availableSpots": 5,
    "isFull": false
  }
]
```

### Book a Class
```http
POST /api/class/book/1
Authorization: Bearer {token}
```

**Response:**
```json
{
  "message": "Class booked successfully!",
  "attendance": {
    "id": 123,
    "classId": 1,
    "userId": "user-id",
    "bookedAt": "2025-08-11T10:30:00",
    "isConfirmed": true,
    "isCheckedIn": false
  }
}
```

### Get My Bookings
```http
GET /api/class/my-bookings
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "id": 123,
    "bookedAt": "2025-08-11T10:30:00",
    "isCheckedIn": false,
    "class": {
      "id": 1,
      "name": "Salsa Beginners",
      "description": "Perfect for first-time dancers",
      "startTime": "2025-08-11T19:00:00",
      "endTime": "2025-08-11T20:00:00",
      "instructorName": "Maria Rodriguez"
    }
  }
]
```

### Check Into Class
```http
POST /api/class/checkin/1
Authorization: Bearer {token}
```

### Cancel Booking
```http
DELETE /api/class/cancel/1
Authorization: Bearer {token}
```

## 🏢 Studio Customization

### Get Site Customization
```http
GET /api/sitecustomization
```

**Response:**
```json
{
  "studioName": "Framboyan Dance Studio",
  "logoUrl": "https://example.com/logo.png",
  "welcomeText": "Welcome to our dance family!",
  "primaryButtonColor": "#e74c3c",
  "backgroundColor": "#ffffff",
  "contactEmail": "info@studio.com",
  "contactPhone": "+1-555-0123"
}
```

## 🔧 Flutter Implementation Examples

### API Client Service
```dart
class ApiClient {
  static const String baseUrl = 'http://localhost:5117';
  final Dio _dio = Dio();

  ApiClient() {
    _dio.options.baseUrl = baseUrl;
    _dio.options.connectTimeout = Duration(seconds: 10);
    _dio.options.receiveTimeout = Duration(seconds: 10);
  }

  // Add JWT token to requests
  void setAuthToken(String token) {
    _dio.options.headers['Authorization'] = 'Bearer $token';
  }

  Future<Response> get(String path) async {
    return await _dio.get(path);
  }

  Future<Response> post(String path, {dynamic data}) async {
    return await _dio.post(path, data: data);
  }
}
```

### Authentication Service
```dart
class AuthService {
  final ApiClient _apiClient = ApiClient();

  Future<LoginResponse> login(String email, String password) async {
    try {
      final response = await _apiClient.post('/api/auth/login', data: {
        'email': email,
        'password': password,
      });

      if (response.statusCode == 200) {
        final loginData = LoginResponse.fromJson(response.data);
        _apiClient.setAuthToken(loginData.token);
        return loginData;
      } else {
        throw ApiException('Login failed');
      }
    } catch (e) {
      throw ApiException('Network error: $e');
    }
  }
}
```

### Class Service
```dart
class ClassService {
  final ApiClient _apiClient = ApiClient();

  Future<List<ClassModel>> getClasses({
    DateTime? startDate,
    DateTime? endDate,
  }) async {
    final params = <String, String>{};
    if (startDate != null) {
      params['startDate'] = startDate.toIso8601String();
    }
    if (endDate != null) {
      params['endDate'] = endDate.toIso8601String();
    }

    final response = await _apiClient.get('/api/class/calendar');
    
    if (response.statusCode == 200) {
      final List<dynamic> data = response.data;
      return data.map((json) => ClassModel.fromJson(json)).toList();
    } else {
      throw ApiException('Failed to load classes');
    }
  }

  Future<void> bookClass(int classId) async {
    await _apiClient.post('/api/class/book/$classId');
  }
}
```

## 🚨 Error Handling

### Common HTTP Status Codes
- **200**: Success
- **400**: Bad Request (validation errors)
- **401**: Unauthorized (invalid/expired token)
- **404**: Not Found
- **500**: Server Error

### Error Response Format
```json
{
  "message": "Error description",
  "errors": ["Specific error 1", "Specific error 2"]
}
```

### Flutter Error Handling
```dart
try {
  final classes = await classService.getClasses();
  // Handle success
} on ApiException catch (e) {
  // Handle API-specific errors
  showErrorDialog(e.message);
} catch (e) {
  // Handle general errors
  showErrorDialog('Something went wrong');
}
```

## 🔄 Real-time Updates

For real-time class updates, consider implementing:
1. **Polling**: Refresh data every 30 seconds
2. **WebSockets**: For instant updates (future enhancement)
3. **Push Notifications**: For booking confirmations

## 🔒 Security Best Practices

1. **Token Storage**: Use secure storage (flutter_secure_storage)
2. **Token Refresh**: Handle token expiration gracefully
3. **HTTPS Only**: Always use HTTPS in production
4. **Input Validation**: Validate all user inputs
5. **Error Messages**: Don't expose sensitive information

---

**This API integration enables seamless communication between the mobile app and your existing backend infrastructure.**
