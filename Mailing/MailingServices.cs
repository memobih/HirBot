using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Mailing
{
    public class MailingService : IMailingService
    {
        private readonly MailSettings _mailSettings;

        public MailingService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public void SendMail(MailMassage message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(MailMassage message)
        {
            var emailMessage = new MimeMessage();
            
            emailMessage.From.Add(new MailboxAddress("HirBot", _mailSettings.From));  // Changed "email" to "sender"
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p><img src=\"cid:logo\" alt=\"CMS\" style=\"vertical-align:middle;\" /> <span style=\"vertical-align:middle;\">Control Management System</span></p>" +
                       $"{message.Content}"
            };



            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage message)
        {
            using (var client = new SmtpClient() )
            {
                try
                {
                    client.Connect(_mailSettings.SmtpServer, _mailSettings.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_mailSettings.Username, _mailSettings.Password);
                    client.Send(message);
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }
    }
}
