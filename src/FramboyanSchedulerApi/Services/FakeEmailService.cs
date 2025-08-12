using System.Threading.Tasks;

namespace FramboyanSchedulerApi.Services
{
    // For demo: logs email to console. Replace with real email sender in production.
    public class FakeEmailService : IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body)
        {
            Console.WriteLine($"EMAIL TO: {toEmail}\nSUBJECT: {subject}\nBODY: {body}");
            return Task.CompletedTask;
        }

        public Task SendWelcomeEmailAsync(string userEmail, string userName)
        {
            Console.WriteLine($"WELCOME EMAIL TO: {userEmail}\nUSER: {userName}");
            return Task.CompletedTask;
        }

        public Task SendPaymentConfirmationAsync(string userEmail, string userName, string paymentDescription, decimal amount, string transactionId)
        {
            Console.WriteLine($"PAYMENT EMAIL TO: {userEmail}\nUSER: {userName}\nPAYMENT: {paymentDescription}\nAMOUNT: ${amount}");
            return Task.CompletedTask;
        }

        public Task SendMembershipActivationAsync(string userEmail, string userName, string membershipType, DateTime startDate, DateTime? endDate, int? classCount)
        {
            Console.WriteLine($"MEMBERSHIP EMAIL TO: {userEmail}\nUSER: {userName}\nTYPE: {membershipType}");
            return Task.CompletedTask;
        }

        public Task SendPasswordResetAsync(string userEmail, string userName, string resetLink)
        {
            Console.WriteLine($"PASSWORD RESET EMAIL TO: {userEmail}\nUSER: {userName}");
            return Task.CompletedTask;
        }

        public Task SendAccountVerificationAsync(string userEmail, string userName, string verificationLink)
        {
            Console.WriteLine($"VERIFICATION EMAIL TO: {userEmail}\nUSER: {userName}");
            return Task.CompletedTask;
        }

        public Task<bool> TestEmailConfigurationAsync()
        {
            Console.WriteLine("EMAIL TEST: Configuration test successful");
            return Task.FromResult(true);
        }
    }
}
