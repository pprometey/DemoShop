using System;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Identity.UsersViewModels
{
    public class EditViewModel : UserViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Display(Name = "Эл. адрес подтвержден?")]
        public bool EmailConfirmed { get; set; }
    }
}