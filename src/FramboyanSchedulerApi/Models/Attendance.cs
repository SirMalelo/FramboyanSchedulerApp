namespace FramboyanSchedulerApi.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public int ClassId { get; set; }
        public DateTime CheckedInAt { get; set; }
    }
}