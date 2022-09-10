using SendGrid.Helpers.Mail;
using SendGrid;

namespace ApiRestAlchemy.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);

    }
    public class SendGridMailService : IMailService
    {
        private IConfiguration _configuration;
        public SendGridMailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {

            var apiKey = _configuration["SendGridAPIKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("grifo010@gmail.com", "Alkemy Api Rest Disney");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }

    }
}
