
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
        
        // Payment Information
        [Range(0, double.MaxValue)]
        public decimal DropInPrice { get; set; } = 0m;
        
        [Range(0, double.MaxValue)]
        public decimal PackagePrice { get; set; } = 0m; // For class packages
        
        [Range(1, 100)]
        public int PackageClassCount { get; set; } = 1; // Number of classes in package
        
        public bool RequiresMembership { get; set; } = false;
        
        public bool AllowDropIn { get; set; } = true;
        
        public bool AllowPackagePurchase { get; set; } = false;
        
        // Computed property for available spots
        public int AvailableSpots => MaxCapacity - Attendances.Count(a => a.IsConfirmed);
    }
}