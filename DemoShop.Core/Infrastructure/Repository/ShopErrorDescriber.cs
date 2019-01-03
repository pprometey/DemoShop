namespace DemoShop.Core.Infrastructure
{
    public class ShopErrorDescriber
    {
        public virtual ShopError DefaultError(string errorText = "An unknown failure has occurred.")
        {
            return new ShopError
            {
                Code = nameof(DefaultError),
                Description = errorText
            };
        }

        public virtual ShopError ConcurrencyFailure()
        {
            return new ShopError
            {
                Code = nameof(ConcurrencyFailure),
                Description = "Optimistic concurrency failure, object has been modified."
            };
        }

        public virtual ShopError NotFoundItem()
        {
            return new ShopError
            {
                Code = nameof(NotFoundItem),
                Description = "Such an element was not found in the database"
            };
        }
    }
}