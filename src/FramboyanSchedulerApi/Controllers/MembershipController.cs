using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FramboyanSchedulerApi.Data;
using FramboyanSchedulerApi.Models;
using Microsoft.AspNetCore.Identity;

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

        // OWNER: Create a payment method
        [HttpPost("create-payment-method")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreatePaymentMethod([FromBody] PaymentMethod method)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _db.PaymentMethods.Add(method);
            await _db.SaveChangesAsync();
            return Ok(method);
        }

        // OWNER: Assign a membership to a user
        [HttpPost("assign-membership")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> AssignMembership([FromBody] AssignMembershipRequest req)
        {
            var user = await _userManager.FindByIdAsync(req.UserId);
            var type = await _db.MembershipTypes.FindAsync(req.MembershipTypeId);
            if (user == null || type == null) return NotFound();
            var membership = new Membership
            {
                UserId = user.Id,
                MembershipTypeId = type.Id,
                StartDate = DateTime.UtcNow,
                EndDate = type.DurationDays.HasValue ? DateTime.UtcNow.AddDays(type.DurationDays.Value) : (DateTime?)null,
                BalanceDue = type.Price,
                IsActive = true
            };
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
                .Where(m => m.UserId == userId).ToListAsync();
            return Ok(memberships);
        }

        // STUDENT: Apply for a membership (creates a pending membership for approval or auto-activates for now)
        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> ApplyMembership([FromBody] ApplyMembershipRequest req)
        {
            var userId = _userManager.GetUserId(User);
            var type = await _db.MembershipTypes.FindAsync(req.MembershipTypeId);
            if (type == null) return NotFound("Membership type not found.");
            var membership = new Membership
            {
                UserId = userId,
                MembershipTypeId = type.Id,
                StartDate = DateTime.UtcNow,
                EndDate = type.DurationDays.HasValue ? DateTime.UtcNow.AddDays(type.DurationDays.Value) : (DateTime?)null,
                BalanceDue = type.Price,
                IsActive = true // For now, auto-activate. Change to false if approval is needed.
            };
            _db.Memberships.Add(membership);
            await _db.SaveChangesAsync();
            return Ok(membership);
        }

        public class AssignMembershipRequest
        {
            public string UserId { get; set; } = string.Empty;
            public int MembershipTypeId { get; set; }
        }

        public class ApplyMembershipRequest
        {
            public int MembershipTypeId { get; set; }
        }
    }
}
