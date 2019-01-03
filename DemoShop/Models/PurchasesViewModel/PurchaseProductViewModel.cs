using System;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.UI.Models.PurchasesViewModel
{
    public class PurchaseProductViewModel
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