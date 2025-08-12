using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FramboyanSchedulerApi.Models;
using FramboyanSchedulerApi.Services;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace FramboyanSchedulerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.ConfirmPassword))
                return BadRequest("Email, password, and confirm password are required.");

            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var user = new ApplicationUser { 
                UserName = model.Email, 
                Email = model.Email, 
                EmailConfirmed = true,
                CreatedBy = "Self-Registered",
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Determine if this is the first user (Owner)
                var users = _userManager.Users.ToList();
                if (users.Count == 1)
                {
                    await _userManager.AddToRoleAsync(user, "Owner");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                }

                // Send welcome email
                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email!, user.FullName ?? user.UserName!);
                }
                catch (Exception ex)
                {
                    // Log error but don't fail registration
                    Console.WriteLine($"Failed to send welcome email: {ex.Message}");
                }

                // Keep existing email code as fallback
                try
                {
                    var smtpHost = _config["Smtp:Host"] ?? Environment.GetEnvironmentVariable("SMTP_HOST");
                    var smtpPort = int.TryParse(_config["Smtp:Port"], out var portVal) ? portVal : int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var envPort) ? envPort : 587;
                    var smtpUser = _config["Smtp:User"] ?? Environment.GetEnvironmentVariable("SMTP_USER");
                    var smtpPass = _config["Smtp:Pass"] ?? Environment.GetEnvironmentVariable("SMTP_PASS");
                    var smtpFrom = _config["Smtp:From"] ?? Environment.GetEnvironmentVariable("SMTP_FROM") ?? smtpUser;
                    var smtpEnableSsl = (_config["Smtp:EnableSsl"] ?? Environment.GetEnvironmentVariable("SMTP_ENABLESSL") ?? "true").ToLower() == "true";

                    var smtpClient = new SmtpClient(smtpHost)
                    {
                        Port = smtpPort,
                        Credentials = new NetworkCredential(smtpUser, smtpPass),
                        EnableSsl = smtpEnableSsl,
                    };
                    var mailMessage = new MailMessage
                    {
                        From = !string.IsNullOrWhiteSpace(smtpFrom) ? new MailAddress(smtpFrom) : new MailAddress("noreply@localhost"),
                        Subject = "Welcome to Framboyan Studio!",
                        Body = "Congrats on your new account. Thanks for joining the studio!",
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(model.Email!);
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email send failed: {ex.Message}");
                }
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Email and password are required.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid email or password.");

            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id),
                new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            };
            foreach (var role in userRoles)
            {
                claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role));
            }
            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["JwtKey"] ?? "super_secret_jwt_key_123!"));
            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                claims: claims,
                expires: System.DateTime.Now.AddDays(7),
                signingCredentials: creds);
            var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { Token = tokenString });
        }

        [HttpDelete("delete-me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> DeleteMe([FromBody] DeleteMeModel model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Email does not match your account.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
                return BadRequest("Password is incorrect.");

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok("Account deleted successfully.");
            return BadRequest(result.Errors);
        }

        public class DeleteMeModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginModel
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        public class RegisterModel
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
            public string? ConfirmPassword { get; set; }
        }

        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(new {
                user.Email,
                user.UserName,
                user.FullName
            });
        }

        [HttpPost("update-profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(model.FullName))
                user.FullName = model.FullName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok();
            return BadRequest(result.Errors);
        }

        public class UpdateProfileModel
        {
            public string? FullName { get; set; }
        }
    }
}
