using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FramboyanSchedulerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStripePaymentIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowOnlineSignup",
                table: "MembershipTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BillingCycleDays",
                table: "MembershipTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MembershipTypes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MembershipTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "MembershipTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresApproval",
                table: "MembershipTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SetupFee",
                table: "MembershipTypes",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MembershipTypes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Memberships",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "Memberships",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingClasses",
                table: "Memberships",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Memberships",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "AllowDropIn",
                table: "Classes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowPackagePurchase",
                table: "Classes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DropInPrice",
                table: "Classes",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PackageClassCount",
                table: "Classes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PackagePrice",
                table: "Classes",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresMembership",
                table: "Classes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PaymentSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AllowMembershipPayments = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowDropInPayments = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowPackagePayments = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequirePaymentForBooking = table.Column<bool>(type: "INTEGER", nullable: false),
                    PaymentDeadlineHours = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowRefunds = table.Column<bool>(type: "INTEGER", nullable: false),
                    RefundDeadlineHours = table.Column<int>(type: "INTEGER", nullable: false),
                    RefundFeePercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    PaymentTerms = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RefundPolicy = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "TEXT", nullable: false),
                    StripeChargeId = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    ProcessingFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    NetAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    PaymentType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MembershipId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClassId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerEmail = table.Column<string>(type: "TEXT", nullable: true),
                    ReceiptUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FailureReason = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StripeSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PublishableKey = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SecretKey = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    WebhookSecret = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ProcessingFeePercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    ProcessingFeeFixed = table.Column<decimal>(type: "TEXT", nullable: false),
                    AdditionalFeePercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    AdditionalFeeFixed = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsLiveMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    BusinessName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    StatementDescriptor = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    SuccessUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CancelUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_ClassId",
                table: "PaymentTransactions",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_MembershipId",
                table: "PaymentTransactions",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_UserId",
                table: "PaymentTransactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentSettings");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "StripeSettings");

            migrationBuilder.DropColumn(
                name: "AllowOnlineSignup",
                table: "MembershipTypes");

            migrationBuilder.DropColumn(
                name: "BillingCycleDays",
                table: "MembershipTypes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MembershipTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MembershipTypes");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "MembershipTypes");

            migrationBuilder.DropColumn(
                name: "RequiresApproval",
                table: "MembershipTypes");

            migrationBuilder.DropColumn(
                name: "SetupFee",
                table: "MembershipTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MembershipTypes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "RemainingClasses",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "AllowDropIn",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "AllowPackagePurchase",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "DropInPrice",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "PackageClassCount",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "PackagePrice",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "RequiresMembership",
                table: "Classes");
        }
    }
}
