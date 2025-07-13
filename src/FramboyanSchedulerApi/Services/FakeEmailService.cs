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
    }
}
