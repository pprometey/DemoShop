using System;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.Goods.ProductsViewModels
{
    public class ProductUnitViewModel
    {
        public Guid ID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Сокращенное наименование")]
        public string ShortName { get; set; }
    }
}