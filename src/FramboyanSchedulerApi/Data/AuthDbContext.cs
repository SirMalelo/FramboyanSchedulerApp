using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FramboyanSchedulerApi.Models;

namespace FramboyanSchedulerApi.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options) { }

        public DbSet<ClassModel> Classes { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<MembershipType> MembershipTypes { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
    }
}