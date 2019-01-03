using DemoShop.Core.Infrastructure;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DemoShop.UI.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAtRegistrationAsync(this IEmailSender emailSender, string email, string fullname, string link)
        {
            return emailSender.SendEmailAsync(new EmailMessage()
            {
                ToEmail = email,
                ToName = fullname,
                Subject = "������������� �����������",
                MessageText = $"������������! <br /> �� ���������������� �� ����� �������������� ����.  <br /> ����������� ����������� � ���� ����� ����������� �����, ������� �� ������: <a href='{HtmlEncoder.Default.Encode(link)}'>����������� �����������</a>"
            });
        }

        public static Task SendEmailConfirmationAtResetPasAsync(this IEmailSender emailSender, string email, string fullname, string link)
        {
            return emailSender.SendEmailAsync(new EmailMessage()
            {
                ToEmail = email,
                ToName = fullname,
                Subject = "�������������� ������",
                MessageText = $"����������, ����������� ����� ������, ������� �� ������: <a href='{HtmlEncoder.Default.Encode(link)}'>�������� ������</a>"
            });
        }

        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string fullname, string link)
        {
            return emailSender.SendEmailAsync(new EmailMessage()
            {
                ToEmail = email,
                ToName = fullname,
                Subject = "����������� ���� ����� ����������� �����",
                MessageText = $"����������� ���� ����� ����������� �����, ������� �� ������: <a href='{HtmlEncoder.Default.Encode(link)}'>����������� ����� ����������� �����</a>"
            });
        }

        public static Task SendEmailConfirmationChangeEmailAsync(this IEmailSender emailSender, string email, string fullname, string link)
        {
            return emailSender.SendEmailAsync(new EmailMessage()
            {
                ToEmail = email,
                ToName = fullname,
                Subject = "����������� ���� ����� ����������� �����",
                MessageText = $"������������! <br /> ��� ����� ������� ������ ��� ������� ����� ����������� �����<br /> ����������� ����������� �����, ������� �� ������: <a href = '{HtmlEncoder.Default.Encode(link)}' > ����������� ����������� �����</a>"
            });
        }
    }
}