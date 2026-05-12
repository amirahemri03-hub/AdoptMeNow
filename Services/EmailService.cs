using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AdoptMeNow.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_settings.SmtpServer)
            {
                Port = _settings.Port,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true,
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            smtpClient.Send(mail);
        }
    }
}