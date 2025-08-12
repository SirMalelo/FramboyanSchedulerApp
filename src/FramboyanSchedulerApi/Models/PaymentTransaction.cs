using System.ComponentModel.DataAnnotations;

namespace FramboyanSchedulerApi.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public ApplicationUser User { get; set; } = null!;
        
        [Required]
        public string StripePaymentIntentId { get; set; } = string.Empty;
        
        [Required]
        public string StripeChargeId { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal ProcessingFee { get; set; }
        
        [Required]
        public decimal NetAmount { get; set; }
        
        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "USD";
        
        [Required]
        [MaxLength(50)]
        public string PaymentType { get; set; } = string.Empty; // "Membership", "DropIn", "Package"
        
        public int? MembershipId { get; set; }
        public Membership? Membership { get; set; }
        
        public int? ClassId { get; set; }
        public ClassModel? Class { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // "Pending", "Completed", "Failed", "Refunded"
        
        public string? Description { get; set; }
        
        public string? CustomerEmail { get; set; }
        
        public string? ReceiptUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? CompletedAt { get; set; }
        
        public string? FailureReason { get; set; }
        
        // Metadata for additional information
        public string? Metadata { get; set; }
    }
}
