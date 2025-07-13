using Microsoft.AspNetCore.Identity;

namespace FramboyanSchedulerApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}