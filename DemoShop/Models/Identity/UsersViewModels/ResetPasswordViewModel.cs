using System;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Identity.UsersViewModels
{
    public class ResetPasswordViewModel : FullNameViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [StringLength(100, ErrorMessage = "{0} должен быть не менее {2} и не больше {1} символов.", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; }
    }
}