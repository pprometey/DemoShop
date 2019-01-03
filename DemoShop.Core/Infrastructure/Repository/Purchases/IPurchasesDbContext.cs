using DemoShop.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace DemoShop.Core.Infrastructure
{
    public interface IPurchasesDbContext : IDbContext
    {
        DbSet<PurchaseInvoice> PurchaseInvoices { get; }
    }
}