using System;
using System.ComponentModel.DataAnnotations;

namespace FramboyanSchedulerApi.Models
{
    public class Membership
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        [Required]
        public int MembershipTypeId { get; set; }
        public MembershipType? MembershipType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal BalanceDue { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
