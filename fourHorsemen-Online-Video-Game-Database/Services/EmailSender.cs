using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace fourHorsemen_Online_Video_Game_Database.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Create the email message and configure the SMTP client
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true
            };

            try
            {
                // Await the SendMailAsync method to ensure it completes before returning
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log the error in case sending email fails
                _logger.LogError($"Failed to send email to {email}. Exception: {ex.Message}");
                throw; // Optional: You can rethrow or handle this as needed
            }
        }
    }
}
