using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ExpenseTracker.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string body)
        {
            var message = new MailMessage
            {
                From = new MailAddress(
                    _settings.SenderEmail,
                    _settings.SenderName),

                Subject = subject,

                Body = body,

                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            using var client = new SmtpClient(
         _settings.SmtpServer,
         _settings.Port);

            client.Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password);

            client.EnableSsl = true;

            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception("SMTP ERROR: " + ex.ToString());
            }
        }
    }
}