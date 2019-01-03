using DemoShop.UI.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoShop.UI.Models.SalesViewModel
{
    public class SaleViewModel
    {
        [Display(Name = "Дата продажи")]
        [DataType(DataType.Date)]
        public DateTime SalesDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [Display(Name = "Товар")]
        [GuidNull(ErrorMessage = "Выберите товар")]
        public Guid ProductID { get; set; }
        public IEnumerable<SalesProductViewModel> Products { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите корректно количество")]
        [Display(Name = "Количество")]
        public int Count { get; set; }

        [Required(ErrorMessage = "Необходимо указать {0}")]
        [Display(Name = "Цена")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

    }
}