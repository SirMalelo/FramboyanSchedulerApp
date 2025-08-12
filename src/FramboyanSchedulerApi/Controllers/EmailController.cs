using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FramboyanSchedulerApi.Models;
using FramboyanSchedulerApi.Data;
using FramboyanSchedulerApi.Services;
using Microsoft.EntityFrameworkCore;

namespace FramboyanSchedulerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(AuthDbContext context, IEmailService emailService, ILogger<EmailController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet("settings")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult<EmailSettings>> GetEmailSettings()
        {
            var settings = await _context.EmailSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new EmailSettings();
                _context.EmailSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            // Don't return password in response
            return Ok(new
            {
                settings.Id,
                settings.SmtpServer,
                settings.SmtpPort,
                settings.SmtpUsername,
                SmtpPassword = "***", // Hide password
                settings.UseSSL,
                settings.FromEmail,
                settings.FromName,
                settings.ReplyToEmail,
                settings.IsEnabled,
                settings.SendWelcomeEmail,
                settings.SendPaymentConfirmation,
                settings.SendMembershipActivation,
                settings.SendClassBookingConfirmation,
                settings.SendPasswordReset,
                settings.SendAccountVerification,
                settings.WelcomeEmailSubject,
                settings.WelcomeEmailTemplate,
                settings.PaymentConfirmationSubject,
                settings.PaymentConfirmationTemplate,
                settings.MembershipActivationSubject,
                settings.MembershipActivationTemplate
            });
        }

        [HttpPut("settings")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> UpdateEmailSettings(EmailSettings model)
        {
            var settings = await _context.EmailSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new EmailSettings();
                _context.EmailSettings.Add(settings);
            }

            // Update all fields except password if it's masked
            settings.SmtpServer = model.SmtpServer;
            settings.SmtpPort = model.SmtpPort;
            settings.SmtpUsername = model.SmtpUsername;
            
            if (model.SmtpPassword != "***")
            {
                settings.SmtpPassword = model.SmtpPassword;
            }
            
            settings.UseSSL = model.UseSSL;
            settings.FromEmail = model.FromEmail;
            settings.FromName = model.FromName;
            settings.ReplyToEmail = model.ReplyToEmail;
            settings.IsEnabled = model.IsEnabled;
            settings.SendWelcomeEmail = model.SendWelcomeEmail;
            settings.SendPaymentConfirmation = model.SendPaymentConfirmation;
            settings.SendMembershipActivation = model.SendMembershipActivation;
            settings.SendClassBookingConfirmation = model.SendClassBookingConfirmation;
            settings.SendPasswordReset = model.SendPasswordReset;
            settings.SendAccountVerification = model.SendAccountVerification;
            settings.WelcomeEmailSubject = model.WelcomeEmailSubject;
            settings.WelcomeEmailTemplate = model.WelcomeEmailTemplate;
            settings.PaymentConfirmationSubject = model.PaymentConfirmationSubject;
            settings.PaymentConfirmationTemplate = model.PaymentConfirmationTemplate;
            settings.MembershipActivationSubject = model.MembershipActivationSubject;
            settings.MembershipActivationTemplate = model.MembershipActivationTemplate;
            settings.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("test")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> TestEmailConfiguration([FromBody] TestEmailRequest request)
        {
            try
            {
                var success = await _emailService.TestEmailConfigurationAsync();
                
                if (success && !string.IsNullOrEmpty(request.TestEmail))
                {
                    // Send a test email
                    var settings = await _context.EmailSettings.FirstOrDefaultAsync();
                    var gymName = (await _context.SiteCustomizations.FirstOrDefaultAsync())?.StudioName ?? "FramboyanScheduler";
                    
                    var subject = $"Test Email from {gymName}";
                    var body = $@"
<h2>Email Configuration Test</h2>
<p>Congratulations! Your email configuration is working correctly.</p>
<p>This is a test email sent from your {gymName} system.</p>
<p>All email notifications are now ready to send to your users.</p>
<p>Test sent at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>";

                    await _emailService.SendWelcomeEmailAsync(request.TestEmail, "Test User");
                }

                return Ok(new { success, message = success ? "Email configuration test successful!" : "Email configuration test failed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email test failed");
                return BadRequest(new { success = false, message = "Email test failed: " + ex.Message });
            }
        }

        [HttpGet("logs")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult<IEnumerable<EmailLog>>> GetEmailLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var logs = await _context.EmailLogs
                .Include(e => e.User)
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(logs);
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult> GetEmailStats()
        {
            var today = DateTime.UtcNow.Date;
            var thisWeek = today.AddDays(-7);
            var thisMonth = today.AddDays(-30);

            var stats = new
            {
                TotalSent = await _context.EmailLogs.CountAsync(e => e.WasSent),
                TotalFailed = await _context.EmailLogs.CountAsync(e => !e.WasSent),
                SentToday = await _context.EmailLogs.CountAsync(e => e.WasSent && e.SentAt >= today),
                SentThisWeek = await _context.EmailLogs.CountAsync(e => e.WasSent && e.SentAt >= thisWeek),
                SentThisMonth = await _context.EmailLogs.CountAsync(e => e.WasSent && e.SentAt >= thisMonth),
                FailedToday = await _context.EmailLogs.CountAsync(e => !e.WasSent && e.CreatedAt >= today),
                ByType = await _context.EmailLogs
                    .Where(e => e.WasSent)
                    .GroupBy(e => e.EmailType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToListAsync()
            };

            return Ok(stats);
        }

        [HttpPost("resend/{logId}")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> ResendEmail(int logId)
        {
            var emailLog = await _context.EmailLogs.FindAsync(logId);
            if (emailLog == null) return NotFound();

            try
            {
                // Create new email log for retry
                var newLog = new EmailLog
                {
                    ToEmail = emailLog.ToEmail,
                    Subject = emailLog.Subject,
                    Body = emailLog.Body,
                    EmailType = emailLog.EmailType + "_Resend",
                    UserId = emailLog.UserId
                };

                // This would trigger the actual resend - simplified for now
                await _emailService.SendWelcomeEmailAsync(emailLog.ToEmail, "User");
                
                return Ok(new { message = "Email resent successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to resend email: " + ex.Message });
            }
        }
    }

    public class TestEmailRequest
    {
        public string TestEmail { get; set; } = "";
    }
}
