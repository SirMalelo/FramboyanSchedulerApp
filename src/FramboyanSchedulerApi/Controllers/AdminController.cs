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
                FullName = s.FullName ?? s.Email,
                EmailConfirmed = s.EmailConfirmed,
                CreatedBy = s.CreatedBy ?? "Self-Registered",
                LockoutEnabled = s.LockoutEnabled
            }).OrderBy(s => s.FullName).ToList();
            
            return Ok(studentList);
        }

        [HttpPost("add-student")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> AddStudent([FromBody] AddStudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest($"A user with email {request.Email} already exists.");

            // Create temporary password if not provided
            var tempPassword = string.IsNullOrEmpty(request.Password) 
                ? GenerateTemporaryPassword() 
                : request.Password;

            var user = new ApplicationUser 
            { 
                UserName = request.Email, 
                Email = request.Email,
                FullName = request.FullName,
                EmailConfirmed = true, // Owner-added students are pre-confirmed
                CreatedBy = "Owner" // Track that this was owner-created
            };

            var result = await _userManager.CreateAsync(user, tempPassword);
            if (result.Succeeded)
            {
                // Assign Student role
                await _userManager.AddToRoleAsync(user, "Student");

                return Ok(new { 
                    message = "Student added successfully!",
                    student = new {
                        user.Id,
                        user.Email,
                        user.FullName,
                        TemporaryPassword = string.IsNullOrEmpty(request.Password) ? tempPassword : null
                    }
                });
            }
            
            return BadRequest(result.Errors);
        }

        [HttpPut("update-student/{studentId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateStudent(string studentId, [FromBody] UpdateStudentRequest request)
        {
            var user = await _userManager.FindByIdAsync(studentId);
            if (user == null)
                return NotFound("Student not found.");

            // Check if user is actually a student
            var isStudent = await _userManager.IsInRoleAsync(user, "Student");
            if (!isStudent)
                return BadRequest("User is not a student.");

            // Update fields
            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                // Check if new email is available
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null && existingUser.Id != studentId)
                    return BadRequest("Email is already in use by another user.");

                user.Email = request.Email;
                user.UserName = request.Email;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { 
                    message = "Student updated successfully!",
                    student = new {
                        user.Id,
                        user.Email,
                        user.FullName
                    }
                });
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("delete-student/{studentId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteStudent(string studentId)
        {
            var user = await _userManager.FindByIdAsync(studentId);
            if (user == null)
                return NotFound("Student not found.");

            // Check if user is actually a student
            var isStudent = await _userManager.IsInRoleAsync(user, "Student");
            if (!isStudent)
                return BadRequest("User is not a student.");

            // TODO: Check if student has active bookings/memberships
            // You might want to prevent deletion if they have future bookings

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok(new { message = $"Student {user.FullName ?? user.Email} deleted successfully." });

            return BadRequest(result.Errors);
        }

        [HttpPost("reset-student-password/{studentId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ResetStudentPassword(string studentId)
        {
            var user = await _userManager.FindByIdAsync(studentId);
            if (user == null)
                return NotFound("Student not found.");

            var isStudent = await _userManager.IsInRoleAsync(user, "Student");
            if (!isStudent)
                return BadRequest("User is not a student.");

            // Generate new temporary password
            var newPassword = GenerateTemporaryPassword();
            
            // Remove old password and set new one
            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
                return BadRequest("Failed to reset password.");

            var addResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (addResult.Succeeded)
            {
                return Ok(new { 
                    message = "Password reset successfully!",
                    temporaryPassword = newPassword 
                });
            }

            return BadRequest(addResult.Errors);
        }

        private string GenerateTemporaryPassword()
        {
            // Generate a secure temporary password
            var random = new Random();
            var chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$";
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public class AddStudentRequest
        {
            public string Email { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string? Password { get; set; } // Optional - will generate temp password if not provided
        }

        public class UpdateStudentRequest
        {
            public string? FullName { get; set; }
            public string? Email { get; set; }
        }
    }
}
