using System;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Goods.ProductsViewModels
{
    public class ProductProductsCategoryViewModel
    {
        public Guid ID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Наименование категории")]
        public string Name { get; set; }

    }
}