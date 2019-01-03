using System.ComponentModel.DataAnnotations;

namespace DemoShop.Core.Domain
{
    public class NamedDbObject : GuidDbObject
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}