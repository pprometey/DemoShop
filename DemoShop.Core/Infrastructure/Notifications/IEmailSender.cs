using System.Threading.Tasks;

namespace DemoShop.Core.Infrastructure
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage message);
    }
}