using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Stripe;
using Stripe.Checkout;
using FramboyanSchedulerApi.Models;
using FramboyanSchedulerApi.Data;
using FramboyanSchedulerApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FramboyanSchedulerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;
        private readonly IEmailService _emailService;

        public PaymentController(AuthDbContext context, IConfiguration configuration, ILogger<PaymentController> logger, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpGet("stripe-settings")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult<StripeSettings>> GetStripeSettings()
        {
            var settings = await _context.StripeSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new StripeSettings();
                _context.StripeSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            // Don't return secret keys in response
            return Ok(new
            {
                settings.Id,
                settings.PublishableKey,
                settings.ProcessingFeePercentage,
                settings.ProcessingFeeFixed,
                settings.AdditionalFeePercentage,
                settings.AdditionalFeeFixed,
                settings.IsLiveMode,
                settings.IsEnabled,
                settings.BusinessName,
                settings.StatementDescriptor
            });
        }

        [HttpPut("stripe-settings")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> UpdateStripeSettings(StripeSettings model)
        {
            var settings = await _context.StripeSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new StripeSettings();
                _context.StripeSettings.Add(settings);
            }

            settings.PublishableKey = model.PublishableKey;
            settings.SecretKey = model.SecretKey;
            settings.WebhookSecret = model.WebhookSecret;
            settings.ProcessingFeePercentage = model.ProcessingFeePercentage;
            settings.ProcessingFeeFixed = model.ProcessingFeeFixed;
            settings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            settings.AdditionalFeeFixed = model.AdditionalFeeFixed;
            settings.IsLiveMode = model.IsLiveMode;
            settings.IsEnabled = model.IsEnabled;
            settings.BusinessName = model.BusinessName;
            settings.StatementDescriptor = model.StatementDescriptor;
            settings.SuccessUrl = model.SuccessUrl;
            settings.CancelUrl = model.CancelUrl;
            settings.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("create-membership-payment")]
        [Authorize]
        public async Task<ActionResult> CreateMembershipPayment([FromBody] CreateMembershipPaymentRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var membershipType = await _context.MembershipTypes.FindAsync(request.MembershipTypeId);
                if (membershipType == null) return NotFound("Membership type not found");

                var stripeSettings = await _context.StripeSettings.FirstOrDefaultAsync();
                if (stripeSettings == null || !stripeSettings.IsEnabled)
                    return BadRequest("Payment processing is not enabled");

                StripeConfiguration.ApiKey = stripeSettings.SecretKey;

                // Calculate total amount including fees
                var subtotal = membershipType.Price + membershipType.SetupFee;
                var processingFee = (subtotal * stripeSettings.ProcessingFeePercentage / 100) + stripeSettings.ProcessingFeeFixed;
                var additionalFee = (subtotal * stripeSettings.AdditionalFeePercentage / 100) + stripeSettings.AdditionalFeeFixed;
                var totalAmount = subtotal + processingFee + additionalFee;

                // Create Stripe Checkout Session
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(totalAmount * 100), // Stripe uses cents
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"{membershipType.Name} Membership",
                                    Description = membershipType.Description
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = stripeSettings.SuccessUrl ?? $"{Request.Scheme}://{Request.Host}/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = stripeSettings.CancelUrl ?? $"{Request.Scheme}://{Request.Host}/payment-cancelled",
                    ClientReferenceId = userId,
                    Metadata = new Dictionary<string, string>
                    {
                        { "payment_type", "membership" },
                        { "membership_type_id", membershipType.Id.ToString() },
                        { "user_id", userId }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                // Create payment transaction record
                var transaction = new PaymentTransaction
                {
                    UserId = userId,
                    StripePaymentIntentId = session.PaymentIntentId ?? "",
                    StripeChargeId = "", // Will be updated by webhook
                    Amount = totalAmount,
                    ProcessingFee = processingFee + additionalFee,
                    NetAmount = subtotal,
                    PaymentType = "Membership",
                    MembershipId = null, // Will be set when membership is created
                    Status = "Pending",
                    Description = $"{membershipType.Name} Membership Purchase",
                    Metadata = System.Text.Json.JsonSerializer.Serialize(new { 
                        membershipTypeId = membershipType.Id,
                        sessionId = session.Id 
                    })
                };

                _context.PaymentTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                return Ok(new { sessionId = session.Id, url = session.Url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating membership payment");
                return StatusCode(500, "Error processing payment");
            }
        }

        [HttpPost("create-class-payment")]
        [Authorize]
        public async Task<ActionResult> CreateClassPayment([FromBody] CreateClassPaymentRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized();

                var classModel = await _context.Classes.FindAsync(request.ClassId);
                if (classModel == null) return NotFound("Class not found");

                if (!classModel.AllowDropIn && request.PaymentType == "DropIn")
                    return BadRequest("Drop-in payments not allowed for this class");

                var stripeSettings = await _context.StripeSettings.FirstOrDefaultAsync();
                if (stripeSettings == null || !stripeSettings.IsEnabled)
                    return BadRequest("Payment processing is not enabled");

                StripeConfiguration.ApiKey = stripeSettings.SecretKey;

                // Calculate amount based on payment type
                decimal subtotal = request.PaymentType == "DropIn" 
                    ? classModel.DropInPrice 
                    : classModel.PackagePrice;

                var processingFee = (subtotal * stripeSettings.ProcessingFeePercentage / 100) + stripeSettings.ProcessingFeeFixed;
                var additionalFee = (subtotal * stripeSettings.AdditionalFeePercentage / 100) + stripeSettings.AdditionalFeeFixed;
                var totalAmount = subtotal + processingFee + additionalFee;

                // Create Stripe Checkout Session
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(totalAmount * 100), // Stripe uses cents
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = request.PaymentType == "DropIn" 
                                        ? $"Drop-in: {classModel.Name}" 
                                        : $"Package: {classModel.Name} ({classModel.PackageClassCount} classes)",
                                    Description = $"{classModel.Name} - {classModel.InstructorName}"
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = stripeSettings.SuccessUrl ?? $"{Request.Scheme}://{Request.Host}/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = stripeSettings.CancelUrl ?? $"{Request.Scheme}://{Request.Host}/payment-cancelled",
                    ClientReferenceId = userId,
                    Metadata = new Dictionary<string, string>
                    {
                        { "payment_type", request.PaymentType.ToLower() },
                        { "class_id", classModel.Id.ToString() },
                        { "user_id", userId }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                // Create payment transaction record
                var transaction = new PaymentTransaction
                {
                    UserId = userId,
                    StripePaymentIntentId = session.PaymentIntentId ?? "",
                    StripeChargeId = "", // Will be updated by webhook
                    Amount = totalAmount,
                    ProcessingFee = processingFee + additionalFee,
                    NetAmount = subtotal,
                    PaymentType = request.PaymentType,
                    ClassId = classModel.Id,
                    Status = "Pending",
                    Description = request.PaymentType == "DropIn" 
                        ? $"Drop-in payment for {classModel.Name}" 
                        : $"Package purchase for {classModel.Name}",
                    Metadata = System.Text.Json.JsonSerializer.Serialize(new { 
                        classId = classModel.Id,
                        sessionId = session.Id 
                    })
                };

                _context.PaymentTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                return Ok(new { sessionId = session.Id, url = session.Url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating class payment");
                return StatusCode(500, "Error processing payment");
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeSettings = await _context.StripeSettings.FirstOrDefaultAsync();
                if (stripeSettings == null) return BadRequest();

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    stripeSettings.WebhookSecret
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    await HandleSuccessfulPayment(session!);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook");
                return BadRequest();
            }
        }

        private async Task HandleSuccessfulPayment(Session session)
        {
            var userId = session.ClientReferenceId;
            var paymentType = session.Metadata["payment_type"];

            // Update payment transaction
            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.Metadata != null && t.Metadata.Contains(session.Id));

            if (transaction != null)
            {
                transaction.Status = "Completed";
                transaction.CompletedAt = DateTime.UtcNow;
                transaction.StripeChargeId = session.PaymentIntentId ?? "";

                // Handle different payment types
                if (paymentType == "membership")
                {
                    await CreateMembershipFromPayment(session, transaction);
                }
                else if (paymentType == "dropin" || paymentType == "package")
                {
                    await CreateClassBookingFromPayment(session, transaction);
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task CreateMembershipFromPayment(Session session, PaymentTransaction transaction)
        {
            var membershipTypeId = int.Parse(session.Metadata["membership_type_id"]);
            var membershipType = await _context.MembershipTypes.FindAsync(membershipTypeId);
            var user = await _context.Users.FindAsync(transaction.UserId);
            
            if (membershipType != null && user != null)
            {
                var membership = new Membership
                {
                    UserId = transaction.UserId,
                    MembershipTypeId = membershipTypeId,
                    StartDate = DateTime.UtcNow,
                    EndDate = membershipType.DurationDays.HasValue 
                        ? DateTime.UtcNow.AddDays(membershipType.DurationDays.Value)
                        : null,
                    RemainingClasses = membershipType.ClassCount,
                    IsActive = true,
                    PurchaseDate = DateTime.UtcNow
                };

                _context.Memberships.Add(membership);
                await _context.SaveChangesAsync();

                transaction.MembershipId = membership.Id;

                // Send email notifications
                try
                {
                    await _emailService.SendPaymentConfirmationAsync(
                        user.Email!,
                        user.FullName ?? user.UserName!,
                        $"{membershipType.Name} Membership",
                        transaction.Amount,
                        transaction.StripeChargeId
                    );

                    await _emailService.SendMembershipActivationAsync(
                        user.Email!,
                        user.FullName ?? user.UserName!,
                        membershipType.Name,
                        membership.StartDate,
                        membership.EndDate,
                        membership.RemainingClasses
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email notifications for membership payment");
                }
            }
        }

        private async Task CreateClassBookingFromPayment(Session session, PaymentTransaction transaction)
        {
            // This would create class bookings/attendance records
            // Implementation depends on your booking system
            var classId = int.Parse(session.Metadata["class_id"]);
            // Add booking logic here
        }

        [HttpGet("transactions")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<ActionResult<IEnumerable<PaymentTransaction>>> GetTransactions()
        {
            return await _context.PaymentTransactions
                .Include(t => t.User)
                .Include(t => t.Membership)
                .Include(t => t.Class)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        [HttpGet("my-transactions")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<PaymentTransaction>>> GetMyTransactions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            return await _context.PaymentTransactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Membership)
                .Include(t => t.Class)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }

    public class CreateMembershipPaymentRequest
    {
        public int MembershipTypeId { get; set; }
    }

    public class CreateClassPaymentRequest
    {
        public int ClassId { get; set; }
        public string PaymentType { get; set; } = "DropIn"; // "DropIn" or "Package"
    }
}
