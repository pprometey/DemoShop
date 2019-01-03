using DemoShop.Core.Domain;
using System;

namespace DemoShop.UI.Models.SalesViewModel
{
    public class EditViewModel : SaleViewModel
    {
        public Guid ID { get; set; }

        public bool IsModifed(SalesInvoice item)
        {
            if (this.ID == item.ID
                && this.SalesDate == item.SalesDate
                && this.ProductID == item.ProductID
                && this.Price == item.Price
                && this.Count == item.Count
            ) { return false; }
            else return true;
        }

    }
}