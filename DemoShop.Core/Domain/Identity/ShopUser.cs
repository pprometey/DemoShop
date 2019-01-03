using Microsoft.AspNetCore.Identity;
using System;

namespace DemoShop.Core.Domain
{
    public class ShopUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
    }
}