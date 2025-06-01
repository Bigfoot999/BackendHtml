using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BackendHtml
{
    public class EmailSender : IEmailSender
    {
        MailSetting mailSetting;
        public EmailSender(IOptions<MailSetting> options)
        {
            this.mailSetting = options.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient smtpClient = new SmtpClient
            {
                Host = mailSetting.Host,
                Port = mailSetting.Port,
                Credentials = new NetworkCredential
                {
                    UserName = mailSetting.Email,
                    Password = mailSetting.Password
                },
                EnableSsl = true
            };
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(mailSetting.Email),
                Subject = subject,
                Body = htmlMessage
            };
            mailMessage.To.Add(new MailAddress(email));
            await smtpClient.SendMailAsync(mailMessage);
        }
    }

}
