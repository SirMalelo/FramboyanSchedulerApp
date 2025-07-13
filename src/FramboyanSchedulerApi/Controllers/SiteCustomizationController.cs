using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FramboyanSchedulerApi.Models;
using System.IO;

namespace FramboyanSchedulerApi.Controllers
{
    [ApiController]
    [Route("api/site/customization")]
    public class SiteCustomizationController : ControllerBase
    {
        private static SiteCustomization _customization = new(); // In-memory for demo; replace with DB in production
        private readonly IWebHostEnvironment _env;
        public SiteCustomizationController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public ActionResult<SiteCustomization> Get() => _customization;

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public IActionResult Save([FromBody] SiteCustomization model)
        {
            _customization.WelcomeText = model.WelcomeText;
            _customization.BackgroundImageUrl = model.BackgroundImageUrl;
            _customization.StudioWebsiteUrl = model.StudioWebsiteUrl;
            return Ok();
        }

        [HttpPost("upload-bg")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UploadBg()
        {
            var file = Request.Form.Files.FirstOrDefault();
            if (file == null) return BadRequest("No file uploaded");
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = $"bg_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var url = $"/uploads/{fileName}";
            _customization.BackgroundImageUrl = url;
            return Ok(url);
        }
    }
}
