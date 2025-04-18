using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace fourHorsemen_Online_Video_Game_Database.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // For now, just log it — later you can plug in SendGrid, SMTP, etc.
            _logger.LogInformation("Sending email to {Email} with subject {Subject}: {Message}", email, subject, htmlMessage);
            return Task.CompletedTask;
        }
    }
}
