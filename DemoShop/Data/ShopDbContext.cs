using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace DemoShop.UI.Data
{
    public class ShopDbContext : IdentityDbContext<ShopUser, ShopRole, Guid>, IGoodsDbContext, IPurchasesDbContext, ISalesDbContext
    {
        public DbSet<ShopModule> Modules { get; set; }

        public DbSet<Unit> Units { get; set; }

        public DbSet<ProductsCategory> ProductsCategories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }

        public DbSet<SalesInvoice> SalesInvoices { get; set; }

        public ShopDbContext(DbContextOptions<ShopDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Unit>()
                .HasIndex(i => i.Name)
                .IsUnique();
        }
    }
}