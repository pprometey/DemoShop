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
                Subject = "Подтверждение регистрации",
                MessageText = $"Здравствуйте! <br /> Вы зарегистрированы на сайте Управленческий учет.  <br /> Подтвердите регистрацию и свой адрес электронной почты, перейдя по ссылке: <a href='{HtmlEncoder.Default.Encode(link)}'>подтвердить регистрацию</a>"
            });
        }

        public static Task SendEmailConfirmationAtResetPasAsync(this IEmailSender emailSender, string email, string fullname, string link)
        {
            return emailSender.SendEmailAsync(new EmailMessage()
            {
                ToEmail = email,
                ToName = fullname,
                Subject = "Восстановление пароля",
                MessageText = $"Пожалуйста, подтвердите сброс пароля, перейдя по ссылке: <a href='{HtmlEncoder.Default.Encode(link)}'>сбросить пароль</a>"
            });
        }

        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string fullname, string link)
        {
            return emailSender.SendEmailAsync(new EmailMessage()
            {
                ToEmail = email,
                ToName = fullname,
                Subject = "Подтвердите свой адрес электронной почты",
                MessageText = $"Подтвердите свой адрес электронной почты, перейдя по ссылке: <a href='{HtmlEncoder.Default.Encode(link)}'>подтвердить адрес электронной почты</a>"
            });
        }

        public static Task SendEmailConfirmationChangeEmailAsync(this IEmailSender emailSender, string email, string fullname, string link)
        {
            return emailSender.SendEmailAsync(new EmailMessage()
            {
                ToEmail = email,
                ToName = fullname,
                Subject = "Подтвердите свой адрес электронной почты",
                MessageText = $"Здравствуйте! <br /> Для Вашей учетной записи был изменен адрес электронной почты<br /> Подтвердите электронный адрес, перейдя по ссылке: <a href = '{HtmlEncoder.Default.Encode(link)}' > подтвердить электронный адрес</a>"
            });
        }
    }
}