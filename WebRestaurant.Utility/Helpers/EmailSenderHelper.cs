using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace WebRestaurant.Utility.Helpers
{
    public static class EmailSenderHelper 
    {
        public static void SendEmail(string fromEmail, string toEmail, string subject, string htmlMessage)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var host = configuration["GMAIL:Host"];
            var port = int.Parse(configuration["GMAIL:Port"]);
            var userName = configuration["GMAIL:Username"];
            var password = configuration["GMAIL:Password"];
            var enable = bool.Parse(configuration["GMAIL:SMTP:starttls:enable"]);
            var smtpClient = new SmtpClient()
            {
                Host = host,
                Port = port,
                EnableSsl = enable,
                Credentials = new NetworkCredential(userName, password),
            };
            var message = new MailMessage(fromEmail, toEmail);
            message.Subject = subject;
            message.Body = htmlMessage;
            message.IsBodyHtml = true;
            smtpClient.Send(message);
        }
    }
}
