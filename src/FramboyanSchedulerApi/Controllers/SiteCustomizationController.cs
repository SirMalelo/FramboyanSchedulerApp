using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FramboyanSchedulerApi.Data;
using FramboyanSchedulerApi.Models;

namespace FramboyanSchedulerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiteCustomizationController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly ILogger<SiteCustomizationController> _logger;
        private readonly IWebHostEnvironment _env;

        public SiteCustomizationController(AuthDbContext context, ILogger<SiteCustomizationController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<SiteCustomization>> GetCustomization()
        {
            try
            {
                var customization = await _context.SiteCustomizations.FirstOrDefaultAsync();
                
                if (customization == null)
                {
                    // Create default customization if none exists
                    customization = new SiteCustomization();
                    _context.SiteCustomizations.Add(customization);
                    await _context.SaveChangesAsync();
                }

                return Ok(customization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting site customization");
                return StatusCode(500, "An error occurred while getting site customization");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateCustomization([FromBody] SiteCustomization customization)
        {
            try
            {
                if (customization == null)
                {
                    return BadRequest("Customization data is required");
                }

                var existingCustomization = await _context.SiteCustomizations.FirstOrDefaultAsync();
                
                if (existingCustomization == null)
                {
                    // Create new if none exists
                    customization.LastUpdated = DateTime.UtcNow;
                    _context.SiteCustomizations.Add(customization);
                }
                else
                {
                    // Update existing
                    existingCustomization.StudioName = customization.StudioName;
                    existingCustomization.LogoUrl = customization.LogoUrl;
                    existingCustomization.FaviconUrl = customization.FaviconUrl;
                    existingCustomization.WelcomeText = customization.WelcomeText;
                    existingCustomization.StudioWebsiteUrl = customization.StudioWebsiteUrl;
                    existingCustomization.ContactEmail = customization.ContactEmail;
                    existingCustomization.ContactPhone = customization.ContactPhone;
                    existingCustomization.Address = customization.Address;
                    
                    // Background & Layout
                    existingCustomization.BackgroundImageUrl = customization.BackgroundImageUrl;
                    existingCustomization.BackgroundColor = customization.BackgroundColor;
                    existingCustomization.BackgroundType = customization.BackgroundType;
                    existingCustomization.GradientStart = customization.GradientStart;
                    existingCustomization.GradientEnd = customization.GradientEnd;
                    existingCustomization.GradientDirection = customization.GradientDirection;
                    
                    // Navigation/Sidebar
                    existingCustomization.SidebarBackgroundColor = customization.SidebarBackgroundColor;
                    existingCustomization.SidebarTextColor = customization.SidebarTextColor;
                    existingCustomization.SidebarHoverColor = customization.SidebarHoverColor;
                    existingCustomization.SidebarActiveColor = customization.SidebarActiveColor;
                    existingCustomization.NavigationStyle = customization.NavigationStyle;
                    
                    // Typography
                    existingCustomization.PrimaryFont = customization.PrimaryFont;
                    existingCustomization.SecondaryFont = customization.SecondaryFont;
                    existingCustomization.HeadingColor = customization.HeadingColor;
                    existingCustomization.TextColor = customization.TextColor;
                    existingCustomization.LinkColor = customization.LinkColor;
                    existingCustomization.LinkHoverColor = customization.LinkHoverColor;
                    
                    // Buttons & Interactive Elements
                    existingCustomization.PrimaryButtonColor = customization.PrimaryButtonColor;
                    existingCustomization.PrimaryButtonTextColor = customization.PrimaryButtonTextColor;
                    existingCustomization.PrimaryButtonHoverColor = customization.PrimaryButtonHoverColor;
                    existingCustomization.SecondaryButtonColor = customization.SecondaryButtonColor;
                    existingCustomization.SecondaryButtonTextColor = customization.SecondaryButtonTextColor;
                    existingCustomization.SecondaryButtonHoverColor = customization.SecondaryButtonHoverColor;
                    existingCustomization.ButtonBorderRadius = customization.ButtonBorderRadius;
                    
                    // Cards & Components
                    existingCustomization.CardBackgroundColor = customization.CardBackgroundColor;
                    existingCustomization.CardBorderColor = customization.CardBorderColor;
                    existingCustomization.CardShadow = customization.CardShadow;
                    existingCustomization.CardBorderRadius = customization.CardBorderRadius;
                    
                    // Forms
                    existingCustomization.InputBackgroundColor = customization.InputBackgroundColor;
                    existingCustomization.InputBorderColor = customization.InputBorderColor;
                    existingCustomization.InputFocusColor = customization.InputFocusColor;
                    existingCustomization.InputTextColor = customization.InputTextColor;
                    
                    // Tables
                    existingCustomization.TableHeaderColor = customization.TableHeaderColor;
                    existingCustomization.TableRowHoverColor = customization.TableRowHoverColor;
                    existingCustomization.TableBorderColor = customization.TableBorderColor;
                    
                    // Custom CSS
                    existingCustomization.CustomCSS = customization.CustomCSS;
                    
                    // Footer
                    existingCustomization.FooterBackgroundColor = customization.FooterBackgroundColor;
                    existingCustomization.FooterTextColor = customization.FooterTextColor;
                    existingCustomization.FooterText = customization.FooterText;
                    
                    // Social Media
                    existingCustomization.FacebookUrl = customization.FacebookUrl;
                    existingCustomization.InstagramUrl = customization.InstagramUrl;
                    existingCustomization.TwitterUrl = customization.TwitterUrl;
                    existingCustomization.YouTubeUrl = customization.YouTubeUrl;
                    
                    // Advanced Settings
                    existingCustomization.EnableAnimations = customization.EnableAnimations;
                    existingCustomization.EnableParticles = customization.EnableParticles;
                    existingCustomization.AnimationSpeed = customization.AnimationSpeed;
                    existingCustomization.ThemePreset = customization.ThemePreset;
                    existingCustomization.MobileOptimized = customization.MobileOptimized;
                    existingCustomization.MobileBreakpoint = customization.MobileBreakpoint;
                    
                    // Navigation Labels
                    existingCustomization.HomeLabel = customization.HomeLabel;
                    existingCustomization.LoginLabel = customization.LoginLabel;
                    existingCustomization.RegisterLabel = customization.RegisterLabel;
                    existingCustomization.ProfileLabel = customization.ProfileLabel;
                    existingCustomization.ClassesLabel = customization.ClassesLabel;
                    existingCustomization.MembershipsLabel = customization.MembershipsLabel;
                    existingCustomization.SiteCustomizationLabel = customization.SiteCustomizationLabel;
                    existingCustomization.MembershipTypesLabel = customization.MembershipTypesLabel;
                    existingCustomization.PaymentMethodsLabel = customization.PaymentMethodsLabel;
                    existingCustomization.AssignMembershipLabel = customization.AssignMembershipLabel;
                    existingCustomization.ManageClassesLabel = customization.ManageClassesLabel;
                    
                    // Tab Visibility
                    existingCustomization.ShowHomeTab = customization.ShowHomeTab;
                    existingCustomization.ShowLoginTab = customization.ShowLoginTab;
                    existingCustomization.ShowRegisterTab = customization.ShowRegisterTab;
                    existingCustomization.ShowProfileTab = customization.ShowProfileTab;
                    existingCustomization.ShowClassesTab = customization.ShowClassesTab;
                    existingCustomization.ShowMembershipsTab = customization.ShowMembershipsTab;
                    existingCustomization.ShowSiteCustomizationTab = customization.ShowSiteCustomizationTab;
                    existingCustomization.ShowMembershipTypesTab = customization.ShowMembershipTypesTab;
                    existingCustomization.ShowPaymentMethodsTab = customization.ShowPaymentMethodsTab;
                    existingCustomization.ShowAssignMembershipTab = customization.ShowAssignMembershipTab;
                    existingCustomization.ShowManageClassesTab = customization.ShowManageClassesTab;
                    
                    // Page Content Labels
                    existingCustomization.MembershipTypesPageTitle = customization.MembershipTypesPageTitle;
                    existingCustomization.PaymentMethodsPageTitle = customization.PaymentMethodsPageTitle;
                    existingCustomization.AssignMembershipPageTitle = customization.AssignMembershipPageTitle;
                    existingCustomization.ClassManagementPageTitle = customization.ClassManagementPageTitle;
                    existingCustomization.StudentMembershipsPageTitle = customization.StudentMembershipsPageTitle;
                    
                    // Button Labels
                    existingCustomization.SaveButtonLabel = customization.SaveButtonLabel;
                    existingCustomization.DeleteButtonLabel = customization.DeleteButtonLabel;
                    existingCustomization.EditButtonLabel = customization.EditButtonLabel;
                    existingCustomization.AddButtonLabel = customization.AddButtonLabel;
                    existingCustomization.CancelButtonLabel = customization.CancelButtonLabel;
                    existingCustomization.BookClassButtonLabel = customization.BookClassButtonLabel;
                    existingCustomization.CheckInButtonLabel = customization.CheckInButtonLabel;
                    
                    existingCustomization.LastUpdated = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return Ok("Site customization updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating site customization");
                return StatusCode(500, "An error occurred while updating site customization");
            }
        }

        [HttpPost("preset/{presetName}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ApplyPreset(string presetName)
        {
            try
            {
                var customization = await _context.SiteCustomizations.FirstOrDefaultAsync();
                if (customization == null)
                {
                    customization = new SiteCustomization();
                    _context.SiteCustomizations.Add(customization);
                }

                switch (presetName.ToLower())
                {
                    case "dark":
                        ApplyDarkTheme(customization);
                        break;
                    case "modern":
                        ApplyModernTheme(customization);
                        break;
                    case "classic":
                        ApplyClassicTheme(customization);
                        break;
                    case "vibrant":
                        ApplyVibrantTheme(customization);
                        break;
                    default:
                        ApplyDefaultTheme(customization);
                        break;
                }

                customization.ThemePreset = presetName;
                customization.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok($"{presetName} theme applied successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying theme preset");
                return StatusCode(500, "An error occurred while applying theme preset");
            }
        }

        [HttpPost("upload-bg")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UploadBg()
        {
            var file = Request.Form.Files.FirstOrDefault();
            if (file == null) return BadRequest("No file uploaded");
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = $"bg_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var url = $"/uploads/{fileName}";
            
            // Update the background image URL in the database
            var customization = await _context.SiteCustomizations.FirstOrDefaultAsync();
            if (customization != null)
            {
                customization.BackgroundImageUrl = url;
                customization.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            
            return Ok(url);
        }

        [HttpGet("css")]
        public async Task<ActionResult<string>> GetGeneratedCSS()
        {
            try
            {
                var customization = await _context.SiteCustomizations.FirstOrDefaultAsync();
                if (customization == null)
                {
                    return Ok(string.Empty);
                }

                var css = GenerateCSS(customization);
                return Ok(css);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating CSS");
                return StatusCode(500, "An error occurred while generating CSS");
            }
        }

        private void ApplyDarkTheme(SiteCustomization customization)
        {
            customization.BackgroundColor = "#1a1a1a";
            customization.TextColor = "#ffffff";
            customization.HeadingColor = "#ffffff";
            customization.SidebarBackgroundColor = "#2d2d2d";
            customization.SidebarTextColor = "#ffffff";
            customization.CardBackgroundColor = "#2d2d2d";
            customization.CardBorderColor = "#404040";
            customization.PrimaryButtonColor = "#007acc";
            customization.FooterBackgroundColor = "#1a1a1a";
        }

        private void ApplyModernTheme(SiteCustomization customization)
        {
            customization.BackgroundColor = "#f8f9fa";
            customization.TextColor = "#495057";
            customization.HeadingColor = "#212529";
            customization.SidebarBackgroundColor = "#ffffff";
            customization.SidebarTextColor = "#495057";
            customization.CardBackgroundColor = "#ffffff";
            customization.CardBorderColor = "#e9ecef";
            customization.PrimaryButtonColor = "#6c757d";
            customization.CardBorderRadius = "12px";
            customization.ButtonBorderRadius = "8px";
        }

        private void ApplyClassicTheme(SiteCustomization customization)
        {
            customization.BackgroundColor = "#ffffff";
            customization.TextColor = "#333333";
            customization.HeadingColor = "#2c3e50";
            customization.SidebarBackgroundColor = "#34495e";
            customization.SidebarTextColor = "#ffffff";
            customization.CardBackgroundColor = "#ffffff";
            customization.CardBorderColor = "#dee2e6";
            customization.PrimaryButtonColor = "#3498db";
            customization.PrimaryFont = "Georgia, serif";
        }

        private void ApplyVibrantTheme(SiteCustomization customization)
        {
            customization.BackgroundType = "gradient";
            customization.GradientStart = "#ff6b6b";
            customization.GradientEnd = "#4ecdc4";
            customization.TextColor = "#ffffff";
            customization.HeadingColor = "#ffffff";
            customization.SidebarBackgroundColor = "#ff6b6b";
            customization.SidebarTextColor = "#ffffff";
            customization.CardBackgroundColor = "rgba(255,255,255,0.9)";
            customization.PrimaryButtonColor = "#ff6b6b";
            customization.EnableAnimations = true;
        }

        private void ApplyDefaultTheme(SiteCustomization customization)
        {
            // Reset to defaults (already set in model defaults)
            customization.BackgroundColor = "#ffffff";
            customization.TextColor = "#333333";
            customization.HeadingColor = "#2c3e50";
            customization.SidebarBackgroundColor = "#2c3e50";
            customization.SidebarTextColor = "#ffffff";
            customization.CardBackgroundColor = "#ffffff";
            customization.PrimaryButtonColor = "#3498db";
        }

        private string GenerateCSS(SiteCustomization customization)
        {
            var css = $@"
/* Site Customization CSS */
:root {{
    --primary-color: {customization.PrimaryButtonColor};
    --secondary-color: {customization.SecondaryButtonColor};
    --text-color: {customization.TextColor};
    --heading-color: {customization.HeadingColor};
    --link-color: {customization.LinkColor};
    --background-color: {customization.BackgroundColor};
    --sidebar-bg: {customization.SidebarBackgroundColor};
    --sidebar-text: {customization.SidebarTextColor};
    --card-bg: {customization.CardBackgroundColor};
    --card-border: {customization.CardBorderColor};
    --input-bg: {customization.InputBackgroundColor};
    --input-border: {customization.InputBorderColor};
    --button-radius: {customization.ButtonBorderRadius};
    --card-radius: {customization.CardBorderRadius};
}}

body {{
    background-color: var(--background-color);
    color: var(--text-color);
    font-family: {customization.PrimaryFont};
}}

{(customization.BackgroundType == "image" && !string.IsNullOrEmpty(customization.BackgroundImageUrl) ? 
    $@"body {{
        background-image: url('{customization.BackgroundImageUrl}');
        background-size: cover;
        background-position: center;
        background-attachment: fixed;
    }}" : "")}

{(customization.BackgroundType == "gradient" ? 
    $@"body {{
        background: linear-gradient({customization.GradientDirection}, {customization.GradientStart}, {customization.GradientEnd});
        min-height: 100vh;
    }}" : "")}

h1, h2, h3, h4, h5, h6 {{
    color: var(--heading-color);
    font-family: {customization.SecondaryFont};
}}

a {{
    color: var(--link-color);
}}

a:hover {{
    color: {customization.LinkHoverColor};
}}

.sidebar {{
    background-color: var(--sidebar-bg) !important;
    color: var(--sidebar-text) !important;
}}

.sidebar .nav-link {{
    color: var(--sidebar-text) !important;
}}

.sidebar .nav-link:hover {{
    background-color: {customization.SidebarHoverColor} !important;
}}

.sidebar .nav-link.active {{
    background-color: {customization.SidebarActiveColor} !important;
}}

.card {{
    background-color: var(--card-bg);
    border: 1px solid var(--card-border);
    border-radius: var(--card-radius);
    box-shadow: {customization.CardShadow};
}}

.btn-primary {{
    background-color: var(--primary-color);
    border-color: var(--primary-color);
    color: {customization.PrimaryButtonTextColor};
    border-radius: var(--button-radius);
}}

.btn-primary:hover {{
    background-color: {customization.PrimaryButtonHoverColor};
    border-color: {customization.PrimaryButtonHoverColor};
}}

.btn-secondary {{
    background-color: var(--secondary-color);
    border-color: var(--secondary-color);
    color: {customization.SecondaryButtonTextColor};
    border-radius: var(--button-radius);
}}

.btn-secondary:hover {{
    background-color: {customization.SecondaryButtonHoverColor};
    border-color: {customization.SecondaryButtonHoverColor};
}}

.form-control {{
    background-color: var(--input-bg);
    border-color: var(--input-border);
    color: {customization.InputTextColor};
}}

.form-control:focus {{
    border-color: {customization.InputFocusColor};
    box-shadow: 0 0 0 0.2rem rgba({HexToRgb(customization.InputFocusColor)}, 0.25);
}}

.table {{
    color: var(--text-color);
}}

.table thead th {{
    background-color: {customization.TableHeaderColor};
    border-color: {customization.TableBorderColor};
}}

.table tbody tr:hover {{
    background-color: {customization.TableRowHoverColor};
}}

.footer {{
    background-color: {customization.FooterBackgroundColor};
    color: {customization.FooterTextColor};
}}

{(customization.EnableAnimations ? @"
.card, .btn, .form-control {{
    transition: all 0.3s ease;
}}

.btn:hover {{
    transform: translateY(-1px);
}}
" : "")}

{customization.CustomCSS}
";

            return css;
        }

        private string HexToRgb(string hex)
        {
            if (string.IsNullOrEmpty(hex) || !hex.StartsWith("#"))
                return "0, 0, 0";

            hex = hex.Substring(1);
            if (hex.Length != 6)
                return "0, 0, 0";

            try
            {
                int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                return $"{r}, {g}, {b}";
            }
            catch
            {
                return "0, 0, 0";
            }
        }
    }
}
