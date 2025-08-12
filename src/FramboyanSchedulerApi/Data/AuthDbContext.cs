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
        public DbSet<SiteCustomization> SiteCustomizations { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<StripeSettings> StripeSettings { get; set; }
    public DbSet<PaymentSettings> PaymentSettings { get; set; }
    public DbSet<EmailSettings> EmailSettings { get; set; }
    public DbSet<EmailLog> EmailLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Class)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Membership>()
                .HasOne(m => m.MembershipType)
                .WithMany()
                .HasForeignKey(m => m.MembershipTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}