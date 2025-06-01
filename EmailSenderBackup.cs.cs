using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BackendHtml
{
    public class EmailSenderBackup : IEmailSender
    {
        IConfiguration configuration;
        public EmailSenderBackup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient smtpClient = new SmtpClient
            {
                Host = configuration["MailSetting:Host"] ?? throw new Exception("Not found MailSetting:Host"),
                Port = Convert.ToInt32(configuration["MailSetting:Port"] ?? throw new Exception("Not found MailSetting:Port")),
                Credentials = new NetworkCredential
                {
                    UserName = configuration["MailSetting:Email"] ?? throw new Exception("Not found MailSetting:Host"),
                    Password = configuration["MailSetting:Password"] ?? throw new Exception("Not found MailSetting:Password")
                },
                EnableSsl = true
            };
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(configuration["MailSetting:Email"] ?? throw new Exception("Not found MailSetting:Email")),
                Subject = subject,
                Body = htmlMessage
            };
            mailMessage.To.Add(new MailAddress(email));
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
