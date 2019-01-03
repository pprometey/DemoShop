using DemoShop.Core.Constants;
using DemoShop.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace DemoShop.Core.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EmailSenderOptions _senderOptions;
        protected IEmailSender _emailSender;

        public EmailSender(IServiceProvider serviceProvider, IOptionsSnapshot<EmailSenderOptions> senderOptions)
        {
            _serviceProvider = serviceProvider;
            _senderOptions = senderOptions.Value;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            switch (_senderOptions.EmailProvider)
            {
                case EmailProviderConstants.SMTP:
                    this._emailSender = _serviceProvider.GetService<SMTPSender>();
                    break;

                case EmailProviderConstants.SparkPosts:
                default:
                    this._emailSender = _serviceProvider.GetService<SparkPostSender>();
                    break;
            }
            await _emailSender.SendEmailAsync(message);
        }
    }
}