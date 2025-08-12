using System.ComponentModel.DataAnnotations;

namespace FramboyanSchedulerApi.Models
{
    public class MembershipType
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal SetupFee { get; set; } = 0m;
        
        public int? ClassCount { get; set; } // e.g., 10 for a 10-class pass, null for unlimited
        
        public int? DurationDays { get; set; } // e.g., 30 for a 1-month membership
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        public bool IsRecurring { get; set; } = false;
        
        [Range(1, 365)]
        public int BillingCycleDays { get; set; } = 30; // Monthly by default
        
        public bool AllowOnlineSignup { get; set; } = true;
        
        public bool RequiresApproval { get; set; } = false;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
