using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Identity.UsersViewModels
{
    public class FullNameViewModel
    {
        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Text)]
        [Display(Name = "Полное имя (ФИО)")]
        public string FullName { get; set; }
    }
}