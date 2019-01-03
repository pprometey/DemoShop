using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Identity.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Необходимо указать {0}")]
        [Display(Name = "Электронный адрес")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        public string Email { get; set; }
    }
}