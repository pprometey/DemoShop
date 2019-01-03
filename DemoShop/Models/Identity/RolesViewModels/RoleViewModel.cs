using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Identity.RolesViewModels
{
    public class RoleViewModel
    {
        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Text)]
        [Display(Name = "Код")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Text)]
        [Display(Name = "Наименование")]
        public string Description { get; set; }

        public SelectList Modules { get; set; }

        [Display(Name = "Категория")]
        [Required(ErrorMessage = "Необходимо выбрать {0}")]
        public Guid ModuleID { get; set; }
    }
}