using DemoShop.UI.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Goods.ProductsCategoriesViewModels
{
    public class ProductsCategoryViewModel
    {
        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Text)]
        [Display(Name = "Наименование категории")]
        public string Name { get; set; }
    }
}