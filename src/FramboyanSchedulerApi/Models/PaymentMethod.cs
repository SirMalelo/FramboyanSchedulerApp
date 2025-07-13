using System.ComponentModel.DataAnnotations;

namespace FramboyanSchedulerApi.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty; // e.g., Apple Pay, Visa, Credit Card
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
