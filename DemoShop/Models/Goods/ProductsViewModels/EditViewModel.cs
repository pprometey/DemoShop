using DemoShop.Core.Domain;
using System;

namespace DemoShop.UI.Models.Goods.ProductsViewModels
{
    public class EditViewModel : ProductViewModel
    {
        public Guid ID { get; set; }

        public bool IsModifed(Product item)
        {
            if (this.ID == item.ID
                && this.Name == item.Name
                && this.ProductsCategoryID == item.ProductsCategoryID
                && this.UnitID == item.UnitID
            ) { return false; }
            else return true;
        }

    }
}