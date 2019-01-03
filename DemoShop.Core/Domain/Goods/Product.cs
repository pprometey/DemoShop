using System;

namespace DemoShop.Core.Domain
{
    public class Product : NamedDbObject, IEquatable<Product>
    {
        public Guid ProductsCategoryID { get; set; }
        public ProductsCategory ProductsCategory { get; set; }

        public Guid UnitID { get; set; }
        public Unit Unit { get; set; }

        public bool Equals(Product other)
        {
            return
                this.Name == other.Name &&
                this.ProductsCategoryID == other.ProductsCategoryID &&
                this.ProductsCategory == other.ProductsCategory &&
                this.UnitID == other.UnitID &&
                this.Unit == other.Unit; 
        }
    }
}