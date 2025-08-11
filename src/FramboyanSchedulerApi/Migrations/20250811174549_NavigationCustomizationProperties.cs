using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FramboyanSchedulerApi.Migrations
{
    /// <inheritdoc />
    public partial class NavigationCustomizationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddButtonLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssignMembershipLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssignMembershipPageTitle",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BookClassButtonLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancelButtonLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CheckInButtonLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClassManagementPageTitle",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClassesLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeleteButtonLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EditButtonLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HomeLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoginLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ManageClassesLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MembershipTypesLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MembershipTypesPageTitle",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MembershipsLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodsLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodsPageTitle",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfileLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegisterLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SaveButtonLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ShowAssignMembershipTab",
                table: "SiteCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowClassesTab",
                table: "SiteCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowHomeTab",
                table: "SiteCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowLoginTab",
                table: "SiteCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowManageClassesTab",
                table: "SiteCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowMembershipTypesTab",
                table: "SiteCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowMembershipsTab",
                table: "SiteCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowPaymentMethodsTab",
                table: "SiteCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowProfileTab",
                table: "SiteCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowRegisterTab",
                table: "SiteCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowSiteCustomizationTab",
                table: "SiteCustomizations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SiteCustomizationLabel",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentMembershipsPageTitle",
                table: "SiteCustomizations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddButtonLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "AssignMembershipLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "AssignMembershipPageTitle",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "BookClassButtonLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "CancelButtonLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "CheckInButtonLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ClassManagementPageTitle",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ClassesLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "DeleteButtonLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "EditButtonLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "HomeLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "LoginLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ManageClassesLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "MembershipTypesLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "MembershipTypesPageTitle",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "MembershipsLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "PaymentMethodsLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "PaymentMethodsPageTitle",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ProfileLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "RegisterLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "SaveButtonLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ShowAssignMembershipTab",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ShowClassesTab",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ShowHomeTab",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ShowLoginTab",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ShowManageClassesTab",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ShowMembershipTypesTab",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ShowMembershipsTab",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ShowPaymentMethodsTab",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ShowProfileTab",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ShowRegisterTab",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "ShowSiteCustomizationTab",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "SiteCustomizationLabel",
                table: "SiteCustomizations");

            migrationBuilder.DropColumn(
                name: "StudentMembershipsPageTitle",
                table: "SiteCustomizations");
        }
    }
}
