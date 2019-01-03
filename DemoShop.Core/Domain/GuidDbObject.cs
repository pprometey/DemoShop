using System;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.Core.Domain
{
    public class GuidDbObject
    {
        [Key]
        [Required]
        public Guid ID { get; set; } = Guid.NewGuid();
    }
}