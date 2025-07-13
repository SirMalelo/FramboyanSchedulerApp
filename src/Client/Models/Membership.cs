namespace Client.Models
{
    public class Membership
    {
        public int Id { get; set; }
        public int MembershipTypeId { get; set; }
        public string MembershipTypeName { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
