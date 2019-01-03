using Newtonsoft.Json;
using System.Collections.Generic;

namespace DemoShop.Core.Domain
{
    public class ProductsCategory : NamedDbObject
    {
        [JsonIgnore]
        public List<Product> Products { get; set; }
    }
}