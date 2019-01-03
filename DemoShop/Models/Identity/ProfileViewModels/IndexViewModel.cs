using DemoShop.Core.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Identity.ProfileViewModels
{
    public class IndexViewModel : ShopStatusViewModel
    {
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        public bool EmailConfirmed { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Text)]
        [Display(Name = "Полное имя (ФИО)")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        [Display(Name = "Электронный адрес")]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Телефонный номер")]
        public string PhoneNumber { get; set; }
    }
}