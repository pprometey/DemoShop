using System;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.Core.Domain
{
    public class SalesInvoice : GuidDbObject
    {
        [Required]
        public DateTime SalesDate { get; set; }

        public Guid ProductID { get; set; }
        public Product Product { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}