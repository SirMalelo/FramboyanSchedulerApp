using System.ComponentModel.DataAnnotations;

namespace FramboyanSchedulerApi.Models
{
    public class MembershipType
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        public int? ClassCount { get; set; } // e.g., 10 for a 10-class pass, null for unlimited
        public int? DurationDays { get; set; } // e.g., 30 for a 1-month membership
        public string? Description { get; set; }
    }
}
