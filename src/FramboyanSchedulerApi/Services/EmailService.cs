using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using FramboyanSchedulerApi.Models;
using FramboyanSchedulerApi.Data;
using Microsoft.EntityFrameworkCore;

namespace FramboyanSchedulerApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly AuthDbContext _context;
        private readonly ILogger<EmailService> _logger;

        public EmailService(AuthDbContext context, ILogger<EmailService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            await SendEmailInternalAsync(toEmail, subject, body, "General", null);
        }

        public async Task SendWelcomeEmailAsync(string userEmail, string userName)
        {
            var settings = await GetEmailSettingsAsync();
            if (settings?.SendWelcomeEmail != true) return;

            var gymName = await GetGymNameAsync();
            var subject = settings.WelcomeEmailSubject
                .Replace("{GymName}", gymName);

            var body = settings.WelcomeEmailTemplate
                .Replace("{UserName}", userName)
                .Replace("{GymName}", gymName);

            await SendEmailInternalAsync(userEmail, subject, body, "Welcome", userName);
        }

        public async Task SendPaymentConfirmationAsync(string userEmail, string userName, string paymentDescription, decimal amount, string transactionId)
        {
            var settings = await GetEmailSettingsAsync();
            if (settings?.SendPaymentConfirmation != true) return;

            var gymName = await GetGymNameAsync();
            var subject = settings.PaymentConfirmationSubject
                .Replace("{GymName}", gymName);

            var body = settings.PaymentConfirmationTemplate
                .Replace("{UserName}", userName)
                .Replace("{GymName}", gymName)
                .Replace("{PaymentDescription}", paymentDescription)
                .Replace("{PaymentAmount}", amount.ToString("F2"))
                .Replace("{TransactionId}", transactionId);

            await SendEmailInternalAsync(userEmail, subject, body, "Payment", userName);
        }

        public async Task SendMembershipActivationAsync(string userEmail, string userName, string membershipType, DateTime startDate, DateTime? endDate, int? classCount)
        {
            var settings = await GetEmailSettingsAsync();
            if (settings?.SendMembershipActivation != true) return;

            var gymName = await GetGymNameAsync();
            var subject = settings.MembershipActivationSubject
                .Replace("{GymName}", gymName);

            var body = settings.MembershipActivationTemplate
                .Replace("{UserName}", userName)
                .Replace("{GymName}", gymName)
                .Replace("{MembershipType}", membershipType)
                .Replace("{StartDate}", startDate.ToString("MMMM dd, yyyy"))
                .Replace("{EndDate}", endDate?.ToString("MMMM dd, yyyy") ?? "No expiration")
                .Replace("{ClassCount}", classCount?.ToString() ?? "Unlimited");

            await SendEmailInternalAsync(userEmail, subject, body, "Membership", userName);
        }

        public async Task SendPasswordResetAsync(string userEmail, string userName, string resetLink)
        {
            var settings = await GetEmailSettingsAsync();
            if (settings?.SendPasswordReset != true) return;

            var gymName = await GetGymNameAsync();
            var subject = $"Password Reset - {gymName}";

            var body = $@"
<h2>Password Reset Request</h2>
<p>Hi {userName},</p>
<p>You requested a password reset for your {gymName} account.</p>
<p><a href='{resetLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Your Password</a></p>
<p>If you didn't request this, please ignore this email.</p>
<p>This link will expire in 24 hours.</p>
<p>Best regards,<br>{gymName} Team</p>";

            await SendEmailInternalAsync(userEmail, subject, body, "PasswordReset", userName);
        }

        public async Task SendAccountVerificationAsync(string userEmail, string userName, string verificationLink)
        {
            var settings = await GetEmailSettingsAsync();
            if (settings?.SendAccountVerification != true) return;

            var gymName = await GetGymNameAsync();
            var subject = $"Verify Your Account - {gymName}";

            var body = $@"
<h2>Account Verification</h2>
<p>Hi {userName},</p>
<p>Please verify your email address to complete your {gymName} account setup.</p>
<p><a href='{verificationLink}' style='background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Verify Email</a></p>
<p>If you didn't create this account, please ignore this email.</p>
<p>Best regards,<br>{gymName} Team</p>";

            await SendEmailInternalAsync(userEmail, subject, body, "Verification", userName);
        }

        public async Task<bool> TestEmailConfigurationAsync()
        {
            try
            {
                var settings = await GetEmailSettingsAsync();
                if (settings == null || !settings.IsEnabled) return false;

                using var client = new SmtpClient();
                await client.ConnectAsync(settings.SmtpServer, settings.SmtpPort, settings.UseSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                await client.AuthenticateAsync(settings.SmtpUsername, settings.SmtpPassword);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email configuration test failed");
                return false;
            }
        }

        private async Task SendEmailInternalAsync(string toEmail, string subject, string body, string emailType, string? userName = null)
        {
            var emailLog = new EmailLog
            {
                ToEmail = toEmail,
                Subject = subject,
                Body = body,
                EmailType = emailType,
                UserId = await GetUserIdByEmailAsync(toEmail)
            };

            try
            {
                var settings = await GetEmailSettingsAsync();
                if (settings == null || !settings.IsEnabled)
                {
                    emailLog.ErrorMessage = "Email settings not configured or disabled";
                    _context.EmailLogs.Add(emailLog);
                    await _context.SaveChangesAsync();
                    return;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(settings.FromName, settings.FromEmail));
                message.To.Add(new MailboxAddress(userName ?? "", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                if (!string.IsNullOrEmpty(settings.ReplyToEmail))
                {
                    message.ReplyTo.Add(new MailboxAddress(settings.FromName, settings.ReplyToEmail));
                }

                using var client = new SmtpClient();
                await client.ConnectAsync(settings.SmtpServer, settings.SmtpPort, settings.UseSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                await client.AuthenticateAsync(settings.SmtpUsername, settings.SmtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                emailLog.WasSent = true;
                emailLog.SentAt = DateTime.UtcNow;

                _logger.LogInformation($"Email sent successfully to {toEmail} with subject: {subject}");
            }
            catch (Exception ex)
            {
                emailLog.ErrorMessage = ex.Message;
                _logger.LogError(ex, $"Failed to send email to {toEmail} with subject: {subject}");
            }

            _context.EmailLogs.Add(emailLog);
            await _context.SaveChangesAsync();
        }

        private async Task<EmailSettings?> GetEmailSettingsAsync()
        {
            return await _context.EmailSettings.FirstOrDefaultAsync();
        }

        private async Task<string> GetGymNameAsync()
        {
            var siteCustomization = await _context.SiteCustomizations.FirstOrDefaultAsync();
            return siteCustomization?.StudioName ?? "FramboyanScheduler";
        }

        private async Task<string?> GetUserIdByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user?.Id;
        }
    }
}
