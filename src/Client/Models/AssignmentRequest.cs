namespace Client.Models
{
    public class AssignmentRequest
    {
        public string UserId { get; set; } = string.Empty;
        public int MembershipTypeId { get; set; }
    }
}
