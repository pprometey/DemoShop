using DemoShop.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace DemoShop.Core.Infrastructure
{
    public interface IGoodsDbContext : IDbContext
    {
        DbSet<Unit> Units { get; }
        DbSet<ProductsCategory> ProductsCategories { get; }
        DbSet<Product> Products { get; }
    }
}