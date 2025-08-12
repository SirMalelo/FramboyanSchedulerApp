// Example mobile-specific endpoints you might want to add:

// GET /api/mobile/app-config
// Returns app configuration (studio name, colors, etc.)
public async Task<IActionResult> GetAppConfig()
{
    var config = await _db.SiteCustomizations.FirstOrDefaultAsync();
    return Ok(new {
        StudioName = config?.StudioName ?? "My Studio",
        PrimaryColor = config?.PrimaryButtonColor ?? "#3498db",
        BackgroundColor = config?.BackgroundColor ?? "#ffffff",
        // Add other mobile-relevant settings
    });
}

// GET /api/mobile/user-profile
// Returns comprehensive user profile for mobile
public async Task<IActionResult> GetUserProfile()
{
    var userId = _userManager.GetUserId(User);
    var user = await _userManager.FindByIdAsync(userId);
    var userRoles = await _userManager.GetRolesAsync(user);
    
    // Get user's membership info
    var membership = await _db.Memberships
        .Where(m => m.UserId == userId && m.IsActive)
        .FirstOrDefaultAsync();
    
    return Ok(new {
        user.Id,
        user.Email,
        user.FullName,
        Roles = userRoles,
        Membership = membership != null ? new {
            membership.MembershipType.Name,
            membership.ExpiryDate,
            membership.IsActive
        } : null,
        TotalBookings = await _db.Attendances.CountAsync(a => a.UserId == userId),
        ClassesAttended = await _db.Attendances.CountAsync(a => a.UserId == userId && a.IsCheckedIn)
    });
}

// POST /api/mobile/device-token
// For push notifications
public async Task<IActionResult> RegisterDeviceToken([FromBody] DeviceTokenRequest request)
{
    // Store device token for push notifications
    // Implementation depends on your push notification service (Firebase, Azure, etc.)
    return Ok();
}
