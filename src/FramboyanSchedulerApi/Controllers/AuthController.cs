using Microsoft.AspNetCore.Mvc;
using FramboyanSchedulerApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace FramboyanSchedulerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Send congratulatory email
                try
                {
                    var smtpClient = new SmtpClient("smtp.yourprovider.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("your@email.com", "yourpassword"),
                        EnableSsl = true,
                    };
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("your@email.com"),
                        Subject = "Welcome to Framboyan Studio!",
                        Body = "Congrats on your new account. Thanks for joining the studio!",
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(model.Email);
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch
                {
                    // Log or handle email sending failure, but do not block registration
                }
                return Ok();
            }
            else
                return BadRequest(result.Errors);
        }
    }

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
