namespace Client.Models
{
    public class ClassBooking
    {
        public int Id { get; set; }
        public DateTime BookedAt { get; set; }
        public bool IsCheckedIn { get; set; }
        public ClassModel Class { get; set; } = new();
    }
}
