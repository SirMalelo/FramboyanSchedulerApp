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
    public class ClassController : ControllerBase
    {
        private readonly AuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClassController(AuthDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // OWNER: Create a new class
        [HttpPost("create")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (req.EndTime <= req.StartTime)
                return BadRequest("End time must be after start time.");

            var classModel = new ClassModel
            {
                Name = req.Name,
                Description = req.Description,
                StartTime = req.StartTime,
                EndTime = req.EndTime,
                MaxCapacity = req.MaxCapacity,
                InstructorName = req.InstructorName,
                IsActive = true
            };

            try
            {
                _db.Classes.Add(classModel);
                await _db.SaveChangesAsync();
                return Ok(classModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create class: {ex.Message}");
            }
        }

        // OWNER: Update a class
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] UpdateClassRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var classModel = await _db.Classes.FindAsync(id);
            if (classModel == null) return NotFound("Class not found.");

            if (req.EndTime <= req.StartTime)
                return BadRequest("End time must be after start time.");

            classModel.Name = req.Name;
            classModel.Description = req.Description;
            classModel.StartTime = req.StartTime;
            classModel.EndTime = req.EndTime;
            classModel.MaxCapacity = req.MaxCapacity;
            classModel.InstructorName = req.InstructorName;

            try
            {
                await _db.SaveChangesAsync();
                return Ok(classModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update class: {ex.Message}");
            }
        }

        // OWNER: Delete a class
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var classModel = await _db.Classes.Include(c => c.Attendances).FirstOrDefaultAsync(c => c.Id == id);
            if (classModel == null) return NotFound("Class not found.");

            if (classModel.Attendances.Any())
                return BadRequest("Cannot delete class with existing bookings. Cancel all bookings first.");

            try
            {
                _db.Classes.Remove(classModel);
                await _db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete class: {ex.Message}");
            }
        }

        // PUBLIC: Get all active classes (calendar view)
        [HttpGet("calendar")]
        public async Task<IActionResult> GetClassCalendar([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var start = startDate ?? DateTime.UtcNow.Date;
            var end = endDate ?? DateTime.UtcNow.Date.AddDays(30);

            var classes = await _db.Classes
                .Where(c => c.IsActive && c.StartTime >= start && c.StartTime <= end)
                .Include(c => c.Attendances)
                .Select(c => new {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.StartTime,
                    c.EndTime,
                    c.MaxCapacity,
                    c.InstructorName,
                    BookedCount = c.Attendances.Count(a => a.IsConfirmed),
                    AvailableSpots = c.MaxCapacity - c.Attendances.Count(a => a.IsConfirmed),
                    IsFull = c.Attendances.Count(a => a.IsConfirmed) >= c.MaxCapacity
                })
                .OrderBy(c => c.StartTime)
                .ToListAsync();

            return Ok(classes);
        }

        // STUDENT: Book a class
        [HttpPost("book/{classId}")]
        [Authorize]
        public async Task<IActionResult> BookClass(int classId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            // Check if user has active membership
            // var hasMembership = await _db.Memberships
            //     .AnyAsync(m => m.UserId == userId && m.IsActive && m.ExpiryDate > DateTime.UtcNow);
            
            // if (!hasMembership)
            //     return BadRequest("You need an active membership to book classes. Please contact the owner to get a membership.");

            var classModel = await _db.Classes
                .Include(c => c.Attendances)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (classModel == null) return NotFound("Class not found.");
            if (!classModel.IsActive) return BadRequest("Class is not active.");
            
            // Use DateTime.Now instead of DateTime.UtcNow for local time comparison
            if (classModel.StartTime <= DateTime.Now) return BadRequest("Cannot book past classes.");

            // Check if already booked
            var existingBooking = await _db.Attendances
                .Where(a => a.UserId == userId && a.ClassId == classId && a.IsConfirmed)
                .FirstOrDefaultAsync();

            if (existingBooking != null)
                return BadRequest("You have already booked this class.");

            // Check capacity
            var currentBookings = classModel.Attendances.Count(a => a.IsConfirmed);
            if (currentBookings >= classModel.MaxCapacity)
                return BadRequest("Class is full.");

            var attendance = new Attendance
            {
                UserId = userId,
                ClassId = classId,
                BookedAt = DateTime.UtcNow,
                IsConfirmed = true,
                IsCheckedIn = false
            };

            try
            {
                _db.Attendances.Add(attendance);
                await _db.SaveChangesAsync();
                return Ok(new { message = "Class booked successfully!", attendance });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to book class: {ex.Message}");
            }
        }

        // STUDENT: Check-in to a class (without prior booking)
        [HttpPost("checkin/{classId}")]
        [Authorize]
        public async Task<IActionResult> CheckInToClass(int classId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            // Check if user has active membership
            // var hasMembership = await _db.Memberships
            //     .AnyAsync(m => m.UserId == userId && m.IsActive && m.ExpiryDate > DateTime.UtcNow);
            
            // if (!hasMembership)
            //     return BadRequest("You need an active membership to check-in to classes. Please contact the owner to get a membership.");

            var classModel = await _db.Classes
                .Include(c => c.Attendances)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (classModel == null) return NotFound("Class not found.");
            if (!classModel.IsActive) return BadRequest("Class is not active.");
            
            // Allow check-in from 30 minutes before class until 30 minutes after start
            var checkInWindow = TimeSpan.FromMinutes(30);
            if (DateTime.Now < classModel.StartTime.Subtract(checkInWindow) || 
                DateTime.Now > classModel.StartTime.Add(checkInWindow))
                return BadRequest("Check-in window is 30 minutes before to 30 minutes after class start time.");

            // Check if already checked in or booked
            var existingAttendance = await _db.Attendances
                .Where(a => a.UserId == userId && a.ClassId == classId)
                .FirstOrDefaultAsync();

            if (existingAttendance != null)
            {
                if (existingAttendance.IsCheckedIn)
                    return BadRequest("You have already checked in to this class.");
                
                // If they had a booking, just mark as checked in
                existingAttendance.IsCheckedIn = true;
                existingAttendance.CheckedInAt = DateTime.UtcNow;
            }
            else
            {
                // Check capacity for walk-ins
                var currentBookings = classModel.Attendances.Count(a => a.IsConfirmed);
                if (currentBookings >= classModel.MaxCapacity)
                    return BadRequest("Class is full. No walk-in spots available.");

                // Create new attendance record for walk-in
                existingAttendance = new Attendance
                {
                    UserId = userId,
                    ClassId = classId,
                    BookedAt = DateTime.UtcNow,
                    IsConfirmed = true,
                    IsCheckedIn = true,
                    CheckedInAt = DateTime.UtcNow
                };
                _db.Attendances.Add(existingAttendance);
            }

            try
            {
                await _db.SaveChangesAsync();
                return Ok(new { message = "Successfully checked in to class!", attendance = existingAttendance });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to check in: {ex.Message}");
            }
        }

        // STUDENT: Cancel booking
        [HttpDelete("cancel/{classId}")]
        [Authorize]
        public async Task<IActionResult> CancelBooking(int classId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            var attendance = await _db.Attendances
                .Where(a => a.UserId == userId && a.ClassId == classId && a.IsConfirmed)
                .FirstOrDefaultAsync();

            if (attendance == null)
                return NotFound("No booking found for this class.");

            var classModel = await _db.Classes.FindAsync(classId);
            if (classModel != null && classModel.StartTime <= DateTime.Now.AddHours(2))
                return BadRequest("Cannot cancel booking less than 2 hours before class start.");

            try
            {
                _db.Attendances.Remove(attendance);
                await _db.SaveChangesAsync();
                return Ok(new { message = "Booking cancelled successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to cancel booking: {ex.Message}");
            }
        }

        // STUDENT: Get my bookings
        [HttpGet("my-bookings")]
        [Authorize]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            var bookings = await _db.Attendances
                .Where(a => a.UserId == userId && a.IsConfirmed)
                .Include(a => a.Class)
                .Select(a => new {
                    a.Id,
                    a.BookedAt,
                    a.IsCheckedIn,
                    Class = new {
                        a.Class!.Id,
                        a.Class.Name,
                        a.Class.Description,
                        a.Class.StartTime,
                        a.Class.EndTime,
                        a.Class.InstructorName
                    }
                })
                .OrderBy(a => a.Class.StartTime)
                .ToListAsync();

            return Ok(bookings);
        }

        // OWNER: Get class bookings
        [HttpGet("bookings/{classId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetClassBookings(int classId)
        {
            var bookings = await _db.Attendances
                .Where(a => a.ClassId == classId && a.IsConfirmed)
                .Include(a => a.User)
                .Select(a => new {
                    a.Id,
                    a.BookedAt,
                    a.IsCheckedIn,
                    User = new {
                        a.User!.Id,
                        a.User.Email,
                        a.User.FullName
                    }
                })
                .OrderBy(a => a.BookedAt)
                .ToListAsync();

            return Ok(bookings);
        }

        public class CreateClassRequest
        {
            [Required]
            public string Name { get; set; } = string.Empty;
            
            public string? Description { get; set; }
            
            [Required]
            public DateTime StartTime { get; set; }
            
            [Required]
            public DateTime EndTime { get; set; }
            
            [Required]
            [Range(1, 100)]
            public int MaxCapacity { get; set; } = 10;
            
            public string? InstructorName { get; set; }
        }

        public class UpdateClassRequest
        {
            [Required]
            public string Name { get; set; } = string.Empty;
            
            public string? Description { get; set; }
            
            [Required]
            public DateTime StartTime { get; set; }
            
            [Required]
            public DateTime EndTime { get; set; }
            
            [Required]
            [Range(1, 100)]
            public int MaxCapacity { get; set; } = 10;
            
            public string? InstructorName { get; set; }
        }
    }
}
