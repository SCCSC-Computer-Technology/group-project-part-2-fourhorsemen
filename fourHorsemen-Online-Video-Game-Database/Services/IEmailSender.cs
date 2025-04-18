namespace fourHorsemen_Online_Video_Game_Database.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
