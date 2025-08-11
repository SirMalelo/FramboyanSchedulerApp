namespace Client.Models
{
    public class SiteCustomization
    {
        public int Id { get; set; }
        
        // Branding
        public string StudioName { get; set; } = "My Fitness Studio";
        public string LogoUrl { get; set; } = string.Empty;
        public string FaviconUrl { get; set; } = string.Empty;
        public string WelcomeText { get; set; } = "Welcome to our studio!";
        public string StudioWebsiteUrl { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        // Background & Layout
        public string BackgroundImageUrl { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = "#ffffff";
        public string BackgroundType { get; set; } = "color"; // "color", "image", "gradient"
        public string GradientStart { get; set; } = "#ffffff";
        public string GradientEnd { get; set; } = "#f0f0f0";
        public string GradientDirection { get; set; } = "to-bottom";
        
        // Navigation/Sidebar
        public string SidebarBackgroundColor { get; set; } = "#2c3e50";
        public string SidebarTextColor { get; set; } = "#ffffff";
        public string SidebarHoverColor { get; set; } = "#34495e";
        public string SidebarActiveColor { get; set; } = "#3498db";
        public string NavigationStyle { get; set; } = "sidebar";
        
        // Typography
        public string PrimaryFont { get; set; } = "Arial, sans-serif";
        public string SecondaryFont { get; set; } = "Arial, sans-serif";
        public string HeadingColor { get; set; } = "#2c3e50";
        public string TextColor { get; set; } = "#333333";
        public string LinkColor { get; set; } = "#3498db";
        public string LinkHoverColor { get; set; } = "#2980b9";
        
        // Buttons & Interactive Elements
        public string PrimaryButtonColor { get; set; } = "#3498db";
        public string PrimaryButtonTextColor { get; set; } = "#ffffff";
        public string PrimaryButtonHoverColor { get; set; } = "#2980b9";
        public string SecondaryButtonColor { get; set; } = "#95a5a6";
        public string SecondaryButtonTextColor { get; set; } = "#ffffff";
        public string SecondaryButtonHoverColor { get; set; } = "#7f8c8d";
        public string ButtonBorderRadius { get; set; } = "4px";
        
        // Cards & Components
        public string CardBackgroundColor { get; set; } = "#ffffff";
        public string CardBorderColor { get; set; } = "#dee2e6";
        public string CardShadow { get; set; } = "0 2px 4px rgba(0,0,0,0.1)";
        public string CardBorderRadius { get; set; } = "8px";
        
        // Forms
        public string InputBackgroundColor { get; set; } = "#ffffff";
        public string InputBorderColor { get; set; } = "#ced4da";
        public string InputFocusColor { get; set; } = "#3498db";
        public string InputTextColor { get; set; } = "#333333";
        
        // Tables
        public string TableHeaderColor { get; set; } = "#f8f9fa";
        public string TableRowHoverColor { get; set; } = "#f5f5f5";
        public string TableBorderColor { get; set; } = "#dee2e6";
        
        // Custom CSS
        public string CustomCSS { get; set; } = string.Empty;
        
        // Footer
        public string FooterBackgroundColor { get; set; } = "#2c3e50";
        public string FooterTextColor { get; set; } = "#ffffff";
        public string FooterText { get; set; } = string.Empty;
        
        // Social Media
        public string FacebookUrl { get; set; } = string.Empty;
        public string InstagramUrl { get; set; } = string.Empty;
        public string TwitterUrl { get; set; } = string.Empty;
        public string YouTubeUrl { get; set; } = string.Empty;
        
        // Advanced Settings
        public bool EnableAnimations { get; set; } = true;
        public bool EnableParticles { get; set; } = false;
        public string AnimationSpeed { get; set; } = "normal";
        
        // Theme Presets
        public string ThemePreset { get; set; } = "default";
        
        // Mobile Responsiveness
        public bool MobileOptimized { get; set; } = true;
        public string MobileBreakpoint { get; set; } = "768px";
        
        // Navigation Labels (Customizable Tab Names)
        public string HomeLabel { get; set; } = "Home";
        public string LoginLabel { get; set; } = "Login";
        public string RegisterLabel { get; set; } = "Create Account";
        public string ProfileLabel { get; set; } = "Profile";
        public string ClassesLabel { get; set; } = "Classes";
        public string MembershipsLabel { get; set; } = "My Memberships";
        public string SiteCustomizationLabel { get; set; } = "Site Customization";
        public string MembershipTypesLabel { get; set; } = "Membership Types";
        public string PaymentMethodsLabel { get; set; } = "Payment Methods";
        public string AssignMembershipLabel { get; set; } = "Assign Membership";
        public string ManageClassesLabel { get; set; } = "Manage Classes";
        
        // Tab Visibility (Owner can hide/show tabs) - Default to true so all tabs show initially
        public bool ShowHomeTab { get; set; } = true;
        public bool ShowLoginTab { get; set; } = true;
        public bool ShowRegisterTab { get; set; } = true;
        public bool ShowProfileTab { get; set; } = true;
        public bool ShowClassesTab { get; set; } = true;
        public bool ShowMembershipsTab { get; set; } = true;
        public bool ShowSiteCustomizationTab { get; set; } = true;
        public bool ShowMembershipTypesTab { get; set; } = true;
        public bool ShowPaymentMethodsTab { get; set; } = true;
        public bool ShowAssignMembershipTab { get; set; } = true;
        public bool ShowManageClassesTab { get; set; } = true;
        
        // Page Content Labels
        public string MembershipTypesPageTitle { get; set; } = "Membership Types";
        public string PaymentMethodsPageTitle { get; set; } = "Payment Methods";
        public string AssignMembershipPageTitle { get; set; } = "Assign Memberships to Students";
        public string ClassManagementPageTitle { get; set; } = "Class Management";
        public string StudentMembershipsPageTitle { get; set; } = "My Memberships";
        
        // Button Labels
        public string SaveButtonLabel { get; set; } = "Save";
        public string DeleteButtonLabel { get; set; } = "Delete";
        public string EditButtonLabel { get; set; } = "Edit";
        public string AddButtonLabel { get; set; } = "Add";
        public string CancelButtonLabel { get; set; } = "Cancel";
        public string BookClassButtonLabel { get; set; } = "Book Class";
        public string CheckInButtonLabel { get; set; } = "Check In";

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
