using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FramboyanSchedulerApi.Migrations
{
    /// <inheritdoc />
    public partial class ComprehensiveSiteCustomization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteCustomizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudioName = table.Column<string>(type: "TEXT", nullable: false),
                    LogoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    FaviconUrl = table.Column<string>(type: "TEXT", nullable: false),
                    WelcomeText = table.Column<string>(type: "TEXT", nullable: false),
                    StudioWebsiteUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ContactEmail = table.Column<string>(type: "TEXT", nullable: false),
                    ContactPhone = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    BackgroundColor = table.Column<string>(type: "TEXT", nullable: false),
                    BackgroundType = table.Column<string>(type: "TEXT", nullable: false),
                    GradientStart = table.Column<string>(type: "TEXT", nullable: false),
                    GradientEnd = table.Column<string>(type: "TEXT", nullable: false),
                    GradientDirection = table.Column<string>(type: "TEXT", nullable: false),
                    SidebarBackgroundColor = table.Column<string>(type: "TEXT", nullable: false),
                    SidebarTextColor = table.Column<string>(type: "TEXT", nullable: false),
                    SidebarHoverColor = table.Column<string>(type: "TEXT", nullable: false),
                    SidebarActiveColor = table.Column<string>(type: "TEXT", nullable: false),
                    NavigationStyle = table.Column<string>(type: "TEXT", nullable: false),
                    PrimaryFont = table.Column<string>(type: "TEXT", nullable: false),
                    SecondaryFont = table.Column<string>(type: "TEXT", nullable: false),
                    HeadingColor = table.Column<string>(type: "TEXT", nullable: false),
                    TextColor = table.Column<string>(type: "TEXT", nullable: false),
                    LinkColor = table.Column<string>(type: "TEXT", nullable: false),
                    LinkHoverColor = table.Column<string>(type: "TEXT", nullable: false),
                    PrimaryButtonColor = table.Column<string>(type: "TEXT", nullable: false),
                    PrimaryButtonTextColor = table.Column<string>(type: "TEXT", nullable: false),
                    PrimaryButtonHoverColor = table.Column<string>(type: "TEXT", nullable: false),
                    SecondaryButtonColor = table.Column<string>(type: "TEXT", nullable: false),
                    SecondaryButtonTextColor = table.Column<string>(type: "TEXT", nullable: false),
                    SecondaryButtonHoverColor = table.Column<string>(type: "TEXT", nullable: false),
                    ButtonBorderRadius = table.Column<string>(type: "TEXT", nullable: false),
                    CardBackgroundColor = table.Column<string>(type: "TEXT", nullable: false),
                    CardBorderColor = table.Column<string>(type: "TEXT", nullable: false),
                    CardShadow = table.Column<string>(type: "TEXT", nullable: false),
                    CardBorderRadius = table.Column<string>(type: "TEXT", nullable: false),
                    InputBackgroundColor = table.Column<string>(type: "TEXT", nullable: false),
                    InputBorderColor = table.Column<string>(type: "TEXT", nullable: false),
                    InputFocusColor = table.Column<string>(type: "TEXT", nullable: false),
                    InputTextColor = table.Column<string>(type: "TEXT", nullable: false),
                    TableHeaderColor = table.Column<string>(type: "TEXT", nullable: false),
                    TableRowHoverColor = table.Column<string>(type: "TEXT", nullable: false),
                    TableBorderColor = table.Column<string>(type: "TEXT", nullable: false),
                    CustomCSS = table.Column<string>(type: "TEXT", nullable: false),
                    FooterBackgroundColor = table.Column<string>(type: "TEXT", nullable: false),
                    FooterTextColor = table.Column<string>(type: "TEXT", nullable: false),
                    FooterText = table.Column<string>(type: "TEXT", nullable: false),
                    FacebookUrl = table.Column<string>(type: "TEXT", nullable: false),
                    InstagramUrl = table.Column<string>(type: "TEXT", nullable: false),
                    TwitterUrl = table.Column<string>(type: "TEXT", nullable: false),
                    YouTubeUrl = table.Column<string>(type: "TEXT", nullable: false),
                    EnableAnimations = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableParticles = table.Column<bool>(type: "INTEGER", nullable: false),
                    AnimationSpeed = table.Column<string>(type: "TEXT", nullable: false),
                    ThemePreset = table.Column<string>(type: "TEXT", nullable: false),
                    MobileOptimized = table.Column<bool>(type: "INTEGER", nullable: false),
                    MobileBreakpoint = table.Column<string>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteCustomizations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteCustomizations");
        }
    }
}
