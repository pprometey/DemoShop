using DemoShop.Core.Domain;
using System;

namespace DemoShop.UI.Models.PurchasesViewModel
{
    public class EditViewModel : PurchaseViewModel
    {
        public Guid ID { get; set; }

        public bool IsModifed(PurchaseInvoice item)
        {
            if (this.ID == item.ID
                && this.PurchaseDate == item.PurchaseDate
                && this.ProductID == item.ProductID
                && this.Price == item.Price
                && this.Count == item.Count
            ) { return false; }
            else return true;
        }

    }
}