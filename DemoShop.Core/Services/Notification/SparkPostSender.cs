using DemoShop.Core.Infrastructure;
using Microsoft.Extensions.Options;
using SparkPostDotNet;
using SparkPostDotNet.Transmissions;
using System.Threading.Tasks;

namespace DemoShop.Core.Services
{
    public class SparkPostSender : IEmailSender
    {
        private SparkPostClient _sparkPostClient;
        private SparkPostSenderOptions _sparkPostSenderOptions;

        public SparkPostSender(SparkPostClient sparkPostClient, IOptions<SparkPostSenderOptions> sparkPostSenderOptions)
        {
            _sparkPostClient = sparkPostClient;
            _sparkPostSenderOptions = sparkPostSenderOptions.Value;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var transmission = new Transmission();

            transmission.Content.From.EMail = _sparkPostSenderOptions.FromEmail;
            transmission.Content.From.Name = _sparkPostSenderOptions.FromName;
            transmission.Content.Subject = message.Subject;
            transmission.Content.Html = message.MessageText;
            transmission.Description = "";

            var recipient = new Recipient();
            recipient.Address.EMail = message.ToEmail;
            recipient.Address.Name = message.ToName;
            transmission.Recipients.Add(recipient);

            await _sparkPostClient.CreateTransmission(transmission);
        }
    }
}