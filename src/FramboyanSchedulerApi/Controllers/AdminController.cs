using Microsoft.AspNetCore.Mvc;
using FramboyanSchedulerApi.Data;
using FramboyanSchedulerApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FramboyanSchedulerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpDelete("delete-user-by-email")]
        public async Task<IActionResult> DeleteUserByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            // Find user by normalized email (case-insensitive)
            var normalizedEmail = email.Trim().ToUpperInvariant();
            var user = _userManager.Users.FirstOrDefault(u => u.NormalizedEmail == normalizedEmail);
            if (user == null)
                return NotFound($"User not found for email: {email}");

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok($"User deleted: {email}");
            else
                return BadRequest(result.Errors);
        }

        [HttpGet("get-user-by-email")]
        public IActionResult GetUserByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var normalizedEmail = email.Trim().ToUpperInvariant();
            var user = _userManager.Users.FirstOrDefault(u => u.NormalizedEmail == normalizedEmail);
            if (user == null)
                return NotFound($"User not found for email: {email}");

            return Ok(new {
                user.Email,
                user.UserName,
                user.Id
                // Add more fields as needed
            });
        }

        [HttpGet("students")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var studentList = students.Select(s => new {
                Id = s.Id,
                Email = s.Email,
                FullName = s.FullName ?? s.Email
            }).ToList();
            
            return Ok(studentList);
        }
    }
}
