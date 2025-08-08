
using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class ClassModel
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public DateTime StartTime { get; set; } = DateTime.Now.AddHours(1);
        
        [Required]
        public DateTime EndTime { get; set; } = DateTime.Now.AddHours(2);
        
        [Required]
        [Range(1, 100)]
        public int MaxCapacity { get; set; } = 10;
        
        public string? InstructorName { get; set; }
        public bool IsActive { get; set; }
        public int BookedCount { get; set; }
        public int AvailableSpots { get; set; }
        public bool IsFull { get; set; }
    }
}