using DemoShop.Core.Infrastructure;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace DemoShop.Core.Services
{
    public class SMTPSender : IEmailSender
    {
        private readonly SMTPSenderOptions _senderOptions;

        public SMTPSender(IOptions<SMTPSenderOptions> senderOptions)
        {
            _senderOptions = senderOptions.Value;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_senderOptions.FromName, _senderOptions.FromEmail));
            emailMessage.To.Add(new MailboxAddress(message.ToName, message.ToEmail));
            emailMessage.InReplyTo = _senderOptions.ReplyTo;
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message.MessageText
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_senderOptions.SMTPHost, _senderOptions.SMTPPort, (SecureSocketOptions)_senderOptions.SecureSocketOptions);
                await client.AuthenticateAsync(_senderOptions.SMTPLogin, _senderOptions.SMTPPassword);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}