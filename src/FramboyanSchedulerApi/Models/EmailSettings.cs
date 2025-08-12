using System.ComponentModel.DataAnnotations;

namespace FramboyanSchedulerApi.Models
{
    public class EmailSettings
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string SmtpServer { get; set; } = "";
        
        [Required]
        [Range(1, 65535)]
        public int SmtpPort { get; set; } = 587;
        
        [Required]
        [MaxLength(200)]
        public string SmtpUsername { get; set; } = "";
        
        [Required]
        [MaxLength(500)]
        public string SmtpPassword { get; set; } = "";
        
        [Required]
        public bool UseSSL { get; set; } = true;
        
        [Required]
        [MaxLength(200)]
        public string FromEmail { get; set; } = "";
        
        [Required]
        [MaxLength(100)]
        public string FromName { get; set; } = "";
        
        [MaxLength(200)]
        public string ReplyToEmail { get; set; } = "";
        
        public bool IsEnabled { get; set; } = true;
        
        // Notification settings
        public bool SendWelcomeEmail { get; set; } = true;
        
        public bool SendPaymentConfirmation { get; set; } = true;
        
        public bool SendMembershipActivation { get; set; } = true;
        
        public bool SendClassBookingConfirmation { get; set; } = true;
        
        public bool SendPasswordReset { get; set; } = true;
        
        public bool SendAccountVerification { get; set; } = true;
        
        // Email templates
        [MaxLength(200)]
        public string WelcomeEmailSubject { get; set; } = "Welcome to {GymName}!";
        
        [MaxLength(2000)]
        public string WelcomeEmailTemplate { get; set; } = @"
<h2>Welcome to {GymName}!</h2>
<p>Hi {UserName},</p>
<p>Thank you for joining our gym! We're excited to have you as a member.</p>
<p>You can now log in to your account and explore our classes and facilities.</p>
<p>Best regards,<br>{GymName} Team</p>";
        
        [MaxLength(200)]
        public string PaymentConfirmationSubject { get; set; } = "Payment Confirmation - {GymName}";
        
        [MaxLength(2000)]
        public string PaymentConfirmationTemplate { get; set; } = @"
<h2>Payment Confirmation</h2>
<p>Hi {UserName},</p>
<p>We've received your payment for: <strong>{PaymentDescription}</strong></p>
<p>Amount: <strong>${PaymentAmount}</strong></p>
<p>Transaction ID: {TransactionId}</p>
<p>Thank you for your business!</p>
<p>Best regards,<br>{GymName} Team</p>";
        
        [MaxLength(200)]
        public string MembershipActivationSubject { get; set; } = "Your Membership is Active!";
        
        [MaxLength(2000)]
        public string MembershipActivationTemplate { get; set; } = @"
<h2>Your Membership is Now Active!</h2>
<p>Hi {UserName},</p>
<p>Great news! Your <strong>{MembershipType}</strong> membership is now active.</p>
<p>Membership Details:</p>
<ul>
<li>Start Date: {StartDate}</li>
<li>End Date: {EndDate}</li>
<li>Classes Included: {ClassCount}</li>
</ul>
<p>You can now book classes and enjoy all membership benefits!</p>
<p>Best regards,<br>{GymName} Team</p>";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class EmailLog
    {
        public int Id { get; set; }
        
        [Required]
        public string ToEmail { get; set; } = "";
        
        [Required]
        public string Subject { get; set; } = "";
        
        [Required]
        public string Body { get; set; } = "";
        
        [Required]
        public string EmailType { get; set; } = ""; // Welcome, Payment, Membership, etc.
        
        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
        
        [Required]
        public bool WasSent { get; set; } = false;
        
        public DateTime? SentAt { get; set; }
        
        public string? ErrorMessage { get; set; }
        
        public int RetryCount { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
