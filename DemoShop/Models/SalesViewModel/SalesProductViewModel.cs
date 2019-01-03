using System;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.SalesViewModel
{
    public class SalesProductViewModel
    {
        public Guid ID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Категория")]
        public string CategoryName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Наименование")]
        public string ProductName { get; set; }
    }
}