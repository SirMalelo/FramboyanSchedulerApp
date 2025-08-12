using System.ComponentModel.DataAnnotations;

namespace FramboyanSchedulerApi.Models
{
    public class StripeSettings
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string PublishableKey { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string SecretKey { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string WebhookSecret { get; set; } = string.Empty;
        
        [Required]
        [Range(0, 100)]
        public decimal ProcessingFeePercentage { get; set; } = 2.9m; // Default Stripe fee
        
        [Required]
        [Range(0, 10)]
        public decimal ProcessingFeeFixed { get; set; } = 0.30m; // Default Stripe fixed fee
        
        [Required]
        [Range(0, 50)]
        public decimal AdditionalFeePercentage { get; set; } = 0m; // Owner's additional fee
        
        [Required]
        [Range(0, 10)]
        public decimal AdditionalFeeFixed { get; set; } = 0m; // Owner's additional fixed fee
        
        [Required]
        public bool IsLiveMode { get; set; } = false; // false = test mode, true = live mode
        
        [Required]
        public bool IsEnabled { get; set; } = true;
        
        [MaxLength(100)]
        public string? BusinessName { get; set; }
        
        [MaxLength(200)]
        public string? StatementDescriptor { get; set; }
        
        [MaxLength(500)]
        public string? SuccessUrl { get; set; }
        
        [MaxLength(500)]
        public string? CancelUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class PaymentSettings
    {
        public int Id { get; set; }
        
        [Required]
        public bool AllowMembershipPayments { get; set; } = true;
        
        [Required]
        public bool AllowDropInPayments { get; set; } = true;
        
        [Required]
        public bool AllowPackagePayments { get; set; } = true;
        
        [Required]
        public bool RequirePaymentForBooking { get; set; } = false;
        
        [Required]
        [Range(0, 24)]
        public int PaymentDeadlineHours { get; set; } = 24; // Hours before class to pay
        
        [Required]
        public bool AllowRefunds { get; set; } = true;
        
        [Required]
        [Range(0, 168)]
        public int RefundDeadlineHours { get; set; } = 24; // Hours before class to get refund
        
        [Required]
        [Range(0, 100)]
        public decimal RefundFeePercentage { get; set; } = 0m; // Refund fee
        
        [MaxLength(500)]
        public string? PaymentTerms { get; set; }
        
        [MaxLength(1000)]
        public string? RefundPolicy { get; set; }
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
