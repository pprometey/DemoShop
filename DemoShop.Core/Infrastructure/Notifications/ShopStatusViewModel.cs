namespace DemoShop.Core.Infrastructure
{
    public abstract class ShopStatusViewModel : IShopStatusViewModel
    {
        public AlertMessage StatusMessage { get; set; }
    }
}