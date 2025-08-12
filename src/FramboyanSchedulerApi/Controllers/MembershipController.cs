using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FramboyanSchedulerApi.Data;
using FramboyanSchedulerApi.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FramboyanSchedulerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipController : ControllerBase
    {
        private readonly AuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FramboyanSchedulerApi.Services.IEmailService _emailService;

        public MembershipController(AuthDbContext db, UserManager<ApplicationUser> userManager, FramboyanSchedulerApi.Services.IEmailService emailService)
        {
            _db = db;
            _userManager = userManager;
            _emailService = emailService;
        }

        // OWNER: Create a new membership type
        [HttpPost("create-type")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateMembershipType([FromBody] MembershipType type)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _db.MembershipTypes.Add(type);
            await _db.SaveChangesAsync();
            return Ok(type);
        }

        // OWNER: Edit a membership type
        [HttpPut("edit-type/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> EditMembershipType(int id, [FromBody] MembershipType updated)
        {
            var type = await _db.MembershipTypes.FindAsync(id);
            if (type == null) return NotFound();
            type.Name = updated.Name;
            type.Price = updated.Price;
            type.ClassCount = updated.ClassCount;
            type.DurationDays = updated.DurationDays;
            type.Description = updated.Description;
            await _db.SaveChangesAsync();
            return Ok(type);
        }

        // OWNER: Delete a membership type
        [HttpDelete("delete-type/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteMembershipType(int id)
        {
            var type = await _db.MembershipTypes.FindAsync(id);
            if (type == null) return NotFound();
            _db.MembershipTypes.Remove(type);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // OWNER: Get all payment methods
        [HttpGet("payment-methods")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var methods = await _db.PaymentMethods.ToListAsync();
            return Ok(methods);
        }

        // OWNER: Create a payment method
        [HttpPost("payment-method")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreatePaymentMethod([FromBody] PaymentMethod method)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _db.PaymentMethods.Add(method);
            await _db.SaveChangesAsync();
            return Ok(method);
        }

        // OWNER: Update a payment method
        [HttpPut("payment-method/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdatePaymentMethod(int id, [FromBody] PaymentMethod updated)
        {
            var method = await _db.PaymentMethods.FindAsync(id);
            if (method == null) return NotFound();
            
            method.Name = updated.Name;
            method.Description = updated.Description;
            method.IsActive = updated.IsActive;
            
            await _db.SaveChangesAsync();
            return Ok(method);
        }

        // OWNER: Delete a payment method
        [HttpDelete("payment-method/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            var method = await _db.PaymentMethods.FindAsync(id);
            if (method == null) return NotFound();
            
            _db.PaymentMethods.Remove(method);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // OWNER: Assign a membership to a user
        [HttpPost("assign")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> AssignMembership([FromBody] AssignMembershipRequest req)
        {
            if (string.IsNullOrEmpty(req.UserId) || req.MembershipTypeId <= 0)
                return BadRequest("Valid UserId and MembershipTypeId are required.");

            var user = await _userManager.FindByIdAsync(req.UserId);
            var type = await _db.MembershipTypes.FindAsync(req.MembershipTypeId);
            if (user == null) return NotFound("User not found.");
            if (type == null) return NotFound("Membership type not found.");

            // Check if user already has an active membership of this type
            var existingMembership = await _db.Memberships
                .Where(m => m.UserId == req.UserId && m.MembershipTypeId == req.MembershipTypeId && m.IsActive)
                .FirstOrDefaultAsync();
            
            if (existingMembership != null)
                return BadRequest("User already has an active membership of this type.");

            var membership = new Membership
            {
                UserId = user.Id,
                MembershipTypeId = type.Id,
                StartDate = DateTime.UtcNow,
                EndDate = type.DurationDays.HasValue ? DateTime.UtcNow.AddDays(type.DurationDays.Value) : (DateTime?)null,
                BalanceDue = type.Price,
                IsActive = true
            };
            
            try
            {
                _db.Memberships.Add(membership);
                await _db.SaveChangesAsync();

                // Simulate payment transaction and send email confirmation
                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    var emailBody = $"Dear {user.FullName ?? user.Email},\n\nYour membership '{type.Name}' has been assigned.\nAmount Due: ${type.Price}\n\nThank you for your purchase!";
                    await _emailService.SendEmailAsync(user.Email, "Membership Confirmation", emailBody);
                }

                return Ok(new { membership, message = !string.IsNullOrWhiteSpace(user.Email) ? "Email confirmation sent." : "No email on file for this user." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to assign membership: {ex.Message}");
            }
        }

        // OWNER: View/search all memberships
        [HttpGet("all-memberships")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetAllMemberships()
        {
            var memberships = await _db.Memberships.Include(m => m.User).Include(m => m.MembershipType).ToListAsync();
            return Ok(memberships);
        }

        // STUDENT: View available membership types
        [HttpGet("types")]
        [Authorize]
        public async Task<IActionResult> GetMembershipTypes()
        {
            var types = await _db.MembershipTypes.ToListAsync();
            return Ok(types);
        }

        // STUDENT: View their own memberships
        [HttpGet("my-memberships")]
        [Authorize]
        public async Task<IActionResult> GetMyMemberships()
        {
            var userId = _userManager.GetUserId(User);
            var memberships = await _db.Memberships.Include(m => m.MembershipType)
                .Where(m => m.UserId == userId)
                .Select(m => new {
                    m.Id,
                    m.MembershipTypeId,
                    MembershipTypeName = m.MembershipType!.Name,
                    m.StartDate,
                    m.EndDate,
                    m.BalanceDue,
                    m.IsActive
                })
                .ToListAsync();
            return Ok(memberships);
        }

        // STUDENT: Apply for a membership (creates a pending membership for approval or auto-activates for now)
        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> ApplyMembership([FromBody] ApplyMembershipRequest req)
        {
            // Only allow membership application through Stripe payment now
            return BadRequest("Memberships must be purchased through the payment system. Please use the purchase membership option.");
        }

        // OWNER: Get all memberships with detailed user information
        [HttpGet("all-memberships-detailed")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetAllMembershipsDetailed()
        {
            var memberships = await _db.Memberships
                .Include(m => m.MembershipType)
                .Include(m => m.User)
                .Select(m => new OwnerMembershipView
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    UserName = m.User.FullName ?? m.User.UserName,
                    UserEmail = m.User.Email,
                    MembershipTypeId = m.MembershipTypeId,
                    MembershipTypeName = m.MembershipType.Name,
                    MembershipPrice = m.MembershipType.Price,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    IsActive = m.IsActive,
                    BalanceDue = m.BalanceDue,
                    RemainingClasses = m.RemainingClasses,
                    CreatedAt = m.CreatedAt
                })
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
                
            return Ok(memberships);
        }

        // OWNER: Suspend a membership
        [HttpPut("suspend/{membershipId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> SuspendMembership(int membershipId)
        {
            var membership = await _db.Memberships.FindAsync(membershipId);
            if (membership == null) return NotFound("Membership not found.");
            
            membership.IsActive = false;
            await _db.SaveChangesAsync();
            
            return Ok(new { message = "Membership suspended successfully." });
        }

        // OWNER: Reactivate a membership
        [HttpPut("reactivate/{membershipId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ReactivateMembership(int membershipId)
        {
            var membership = await _db.Memberships.FindAsync(membershipId);
            if (membership == null) return NotFound("Membership not found.");
            
            membership.IsActive = true;
            await _db.SaveChangesAsync();
            
            return Ok(new { message = "Membership reactivated successfully." });
        }

        public class AssignMembershipRequest
        {
            [Required]
            public string UserId { get; set; } = string.Empty;
            
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "MembershipTypeId must be greater than 0")]
            public int MembershipTypeId { get; set; }
        }

        public class ApplyMembershipRequest
        {
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "MembershipTypeId must be greater than 0")]
            public int MembershipTypeId { get; set; }
        }

        public class OwnerMembershipView
        {
            public int Id { get; set; }
            public string UserId { get; set; } = string.Empty;
            public string? UserName { get; set; }
            public string? UserEmail { get; set; }
            public int MembershipTypeId { get; set; }
            public string MembershipTypeName { get; set; } = string.Empty;
            public decimal MembershipPrice { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool IsActive { get; set; }
            public decimal BalanceDue { get; set; }
            public int? RemainingClasses { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
