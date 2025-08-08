
using System.ComponentModel.DataAnnotations;

namespace FramboyanSchedulerApi.Models
{
    public class ClassModel
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public DateTime StartTime { get; set; }
        
        [Required]
        public DateTime EndTime { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100")]
        public int MaxCapacity { get; set; } = 10;
        
        public string? InstructorName { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public List<Attendance> Attendances { get; set; } = new();
        
        // Computed property for available spots
        public int AvailableSpots => MaxCapacity - Attendances.Count(a => a.IsConfirmed);
    }
}