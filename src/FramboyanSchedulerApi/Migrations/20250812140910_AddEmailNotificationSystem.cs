using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FramboyanSchedulerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailNotificationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ToEmail = table.Column<string>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: false),
                    EmailType = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    WasSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    SentAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmailSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SmtpServer = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SmtpPort = table.Column<int>(type: "INTEGER", nullable: false),
                    SmtpUsername = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SmtpPassword = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    UseSSL = table.Column<bool>(type: "INTEGER", nullable: false),
                    FromEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    FromName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ReplyToEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendWelcomeEmail = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendPaymentConfirmation = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendMembershipActivation = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendClassBookingConfirmation = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendPasswordReset = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendAccountVerification = table.Column<bool>(type: "INTEGER", nullable: false),
                    WelcomeEmailSubject = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    WelcomeEmailTemplate = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    PaymentConfirmationSubject = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PaymentConfirmationTemplate = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    MembershipActivationSubject = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MembershipActivationTemplate = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_UserId",
                table: "EmailLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "EmailSettings");
        }
    }
}
