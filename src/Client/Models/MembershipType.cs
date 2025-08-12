namespace Client.Models
{
    public class MembershipType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal SetupFee { get; set; } = 0m;
        public int? ClassCount { get; set; }
        public int? DurationDays { get; set; }
        public string? Description { get; set; }
        public bool IsRecurring { get; set; } = false;
        public int BillingCycleDays { get; set; } = 30;
        public bool AllowOnlineSignup { get; set; } = true;
        public bool RequiresApproval { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
