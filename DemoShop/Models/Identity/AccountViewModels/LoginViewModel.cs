using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Identity.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Необходимо указать {0}")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}