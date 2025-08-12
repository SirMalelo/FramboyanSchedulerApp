namespace FramboyanSchedulerApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendWelcomeEmailAsync(string userEmail, string userName);
        Task SendPaymentConfirmationAsync(string userEmail, string userName, string paymentDescription, decimal amount, string transactionId);
        Task SendMembershipActivationAsync(string userEmail, string userName, string membershipType, DateTime startDate, DateTime? endDate, int? classCount);
        Task SendPasswordResetAsync(string userEmail, string userName, string resetLink);
        Task SendAccountVerificationAsync(string userEmail, string userName, string verificationLink);
        Task<bool> TestEmailConfigurationAsync();
    }
}
