using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Identity.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Необходимо указать {0}")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [StringLength(100, ErrorMessage = "{0} должен быть не менее {2} и не больше {1} символов.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароль и подтверждение пароля не совпадают.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}