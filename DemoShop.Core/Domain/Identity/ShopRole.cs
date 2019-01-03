using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace DemoShop.Core.Domain
{
    public class ShopRole : IdentityRole<Guid>
    {
        public string Description { get; set; }
        public Guid ModuleID { get; set; }
        public ShopModule Module { get; set; }

        public ShopRole() : base()
        {
        }

        public ShopRole(string roleName) : base(roleName)
        {
        }
    }

    public class ShopModule
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}