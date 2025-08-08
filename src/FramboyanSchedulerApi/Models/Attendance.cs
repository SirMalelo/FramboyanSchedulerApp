using System.ComponentModel.DataAnnotations;

namespace FramboyanSchedulerApi.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int ClassId { get; set; }
        
        public DateTime BookedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? CheckedInAt { get; set; }
        
        public bool IsConfirmed { get; set; } = true;
        
        public bool IsCheckedIn { get; set; } = false;
        
        // Navigation properties
        public ApplicationUser? User { get; set; }
        public ClassModel? Class { get; set; }
    }
}