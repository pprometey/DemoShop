using Microsoft.AspNetCore.Mvc;

namespace DemoShop.Core.Infrastructure
{
    public interface IShopController
    {
        [TempData]
        string StatusMessage { get; set; }
    }
}