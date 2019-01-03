using Microsoft.AspNetCore.Mvc;

namespace DemoShop.Core.Infrastructure
{
    public abstract class ShopController : Controller, IShopController
    {
        [TempData]
        public string StatusMessage { get; set; }
    }
}