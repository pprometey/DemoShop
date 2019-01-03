using DemoShop.UI.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Goods.ProductsViewModels
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Необходимо указать {0}")]
        [DataType(DataType.Text)]
        [Display(Name = "Наименование товара")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [Display(Name = "Единица измерения")]
        [GuidNull(ErrorMessage = "Выберите единицу измерения")]
        public Guid UnitID { get; set; }
        public IEnumerable<ProductUnitViewModel> Units { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [Display(Name = "Категория товара")]
        [GuidNull(ErrorMessage = "Выберите категорию товара")]
        public Guid ProductsCategoryID { get; set; }
        public IEnumerable<ProductProductsCategoryViewModel> ProductsCategories { get; set; }

    }
}