# 🧪 FramboyanScheduler - Complete Testing Guide

## 📋 Checkpoint Status
- ✅ **All features implemented**
- ✅ **Build system working**
- ✅ **Azure deployment ready**
- ✅ **Authentication system complete**
- ✅ **Database migrations ready**

---

## 🚀 Local Testing Setup

### 1. Start the Applications
```powershell
# Terminal 1 - Start API
cd src/FramboyanSchedulerApi
dotnet run

# Terminal 2 - Start Client  
cd src/Client
dotnet run
```

**Expected URLs:**
- 🔧 **API**: `https://localhost:7123` (or check console output)
- 💻 **Client**: `https://localhost:7156` (or check console output)

### 2. Verify Health Check
Visit: `https://localhost:7123/health`
**Expected**: `Healthy` response

---

## 🎯 Feature Testing Checklist

### **Phase 1: Authentication & Registration**

#### ✅ **User Registration**
1. Go to client URL
2. Click **"Register"**
3. Fill out form:
   - **Email**: `test@example.com`
   - **Password**: `Test123!`
   - **Full Name**: `Test User`
4. **Expected**: Successful registration + automatic login

#### ✅ **User Login**
1. Click **"Login"**
2. Enter credentials from registration
3. **Expected**: Successful login with JWT token

#### ✅ **User Logout**
1. Click user menu/logout button
2. **Expected**: Redirected to login, token cleared

---

### **Phase 2: Profile Management**

#### ✅ **View Profile**
1. Login as registered user
2. Navigate to **"Profile"** page
3. **Expected**: Shows user information, membership status

#### ✅ **Complete Profile**
1. Go to **"Complete Profile"** page
2. Update additional user information
3. **Expected**: Profile updated successfully

---

### **Phase 3: Class Management (Student View)**

#### ✅ **View Classes**
1. Navigate to **"Classes"** page
2. **Expected**: List of available classes (may be empty initially)

#### ✅ **Class Details**
1. Click on a class (if any exist)
2. **Expected**: Shows class details, schedule, instructor info

---

### **Phase 4: Admin/Owner Functions**

#### ✅ **Access Admin Panel**
1. Login with owner/admin account
2. **Expected**: Additional menu options for admin functions

#### ✅ **Membership Types Management**
1. Go to **"Owner Membership Types"**
2. Test:
   - Create new membership type
   - Edit existing type
   - Delete type
3. **Expected**: CRUD operations work correctly

#### ✅ **Payment Methods**
1. Go to **"Owner Payment Methods"**
2. Test:
   - Add payment method
   - Edit payment method
   - Remove payment method
3. **Expected**: Payment methods managed successfully

#### ✅ **Assign Memberships**
1. Go to **"Owner Assign Membership"**
2. Test:
   - Select user
   - Assign membership type
   - Set dates/pricing
3. **Expected**: Membership assigned to user

#### ✅ **Site Customization**
1. Go to **"Owner Site Customization"**
2. Test:
   - Update site name
   - Change branding
   - Modify settings
3. **Expected**: Site appearance/settings updated

---

### **Phase 5: Student Membership Management**

#### ✅ **View My Memberships**
1. Login as student
2. Go to **"Student Memberships"**
3. **Expected**: Shows assigned memberships, status, expiry

---

### **Phase 6: Authorization Testing**

#### ✅ **Role-Based Access**
1. **As Student**: Try to access owner pages
   - **Expected**: Access denied/redirected
2. **As Owner**: Access all pages
   - **Expected**: Full access to admin functions

#### ✅ **Authentication Required**
1. **Logged Out**: Try to access protected pages
   - **Expected**: Redirected to login

---

## 🔧 Technical Testing

### **API Endpoints**
Test these endpoints with a tool like Postman:

#### **Authentication**
- `POST /api/auth/register`
- `POST /api/auth/login`

#### **Admin Functions**
- `GET /api/admin/users` (requires admin)
- `GET /api/admin/memberships`

#### **Membership Management**
- `GET /api/membership/types`
- `POST /api/membership/assign`
- `GET /api/membership/user/{userId}`

#### **Site Customization**
- `GET /api/sitecustomization`
- `PUT /api/sitecustomization`

### **Database Testing**
1. Check SQLite database: `src/FramboyanSchedulerApi/framboyan.db`
2. Verify tables are created:
   - AspNetUsers
   - AspNetRoles
   - Classes
   - Memberships
   - MembershipTypes
   - PaymentMethods
   - SiteCustomization

### **Security Testing**
1. **JWT Token**: Verify tokens are generated and validated
2. **CORS**: Test cross-origin requests work correctly
3. **HTTPS**: Ensure SSL/TLS is enforced
4. **Authorization**: Verify role-based access control

---

## 🐛 Common Issues & Solutions

### **Issue**: CORS errors
**Solution**: Check API CORS configuration allows client URL

### **Issue**: 401 Unauthorized
**Solution**: Verify JWT token is being sent in headers

### **Issue**: Database errors
**Solution**: Delete `framboyan.db` file and restart API (recreates DB)

### **Issue**: Build errors
**Solution**: Run `dotnet clean` then `dotnet restore` in both projects

---

## 📊 Performance Testing

### **Load Testing (Optional)**
1. Use tools like Apache Bench or Postman collections
2. Test login endpoint with multiple concurrent users
3. Test class listing with database queries

### **Browser Compatibility**
Test in:
- ✅ Chrome
- ✅ Firefox  
- ✅ Edge
- ✅ Safari (if available)

---

## 🎉 Test Scenarios

### **Scenario 1: New Gym Owner**
1. Register as new user
2. System should detect first user = owner
3. Access all admin functions
4. Set up gym (site customization)
5. Create membership types
6. Add payment methods

### **Scenario 2: Student Registration**
1. Register as student
2. Owner assigns membership
3. Student views membership status
4. Student browses available classes

### **Scenario 3: Class Management**
1. Owner creates classes
2. Sets schedules and capacity
3. Students can view and potentially book classes

---

## ✅ Testing Completion Checklist

- [ ] Registration works
- [ ] Login/logout works
- [ ] Profile management works
- [ ] Admin panel accessible (for owners)
- [ ] Membership types CRUD
- [ ] Payment methods CRUD
- [ ] Membership assignment works
- [ ] Site customization works
- [ ] Student membership view works
- [ ] Authorization rules enforced
- [ ] Database operations successful
- [ ] API endpoints respond correctly
- [ ] No console errors
- [ ] Responsive design works on mobile
- [ ] SSL/Security headers present

---

## 🚀 Ready for Production?

After completing all tests:
- ✅ **Local testing complete**
- ✅ **All features working**
- ✅ **Security verified**
- ✅ **Performance acceptable**

**→ Ready for Azure deployment!**

---

## 🆘 Need Help?

If any tests fail:
1. Check browser console for errors
2. Check API logs in terminal
3. Verify database has correct data
4. Test individual API endpoints
5. Clear browser cache/cookies

**Your FramboyanScheduler is ready for comprehensive testing!** 🎯
