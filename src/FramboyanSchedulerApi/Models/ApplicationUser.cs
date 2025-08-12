using Microsoft.AspNetCore.Identity;

namespace FramboyanSchedulerApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? CreatedBy { get; set; } // Track if created by "Owner" or "Self-Registered"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}