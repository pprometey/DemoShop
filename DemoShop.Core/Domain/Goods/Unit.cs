using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.Core.Domain
{
    public class Unit : NamedDbObject
    {
        [Required]
        [MaxLength(20)]
        public string ShortName { get; set; }

        [JsonIgnore]
        public List<Product> Products { get; set; }
    }
}