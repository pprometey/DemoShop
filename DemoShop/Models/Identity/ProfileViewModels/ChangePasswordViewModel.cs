using DemoShop.Core.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Identity.ProfileViewModels
{
    public class ChangePasswordViewModel : ShopStatusViewModel
    {
        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        [StringLength(100, ErrorMessage = "{0} должен быть не менее {2} и не больше {1} символов.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите новый пароль")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и подтверждение пароля не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}