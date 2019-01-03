using DemoShop.Core.Domain;

namespace DemoShop.Core.Infrastructure
{
    public interface IGoodsDbRepositories
    {
        IDbRepository<Unit> Units { get; }
        IDbRepository<ProductsCategory> ProductsCategories { get; }
        IDbRepository<Product> Products { get; }
    }
}