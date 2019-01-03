using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Identity.UsersViewModels
{
    public class UserViewModel : SmallUserViewModel
    {
        [Required(ErrorMessage = "Необходимо указать {0}")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Телефонный номер")]
        public string PhoneNumber { get; set; }
    }
}