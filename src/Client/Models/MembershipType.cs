namespace Client.Models
{
    public class MembershipType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int? ClassCount { get; set; }
        public int? DurationDays { get; set; }
        public string? Description { get; set; }
    }
}
