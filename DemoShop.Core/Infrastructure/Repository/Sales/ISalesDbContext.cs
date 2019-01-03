using DemoShop.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace DemoShop.Core.Infrastructure
{
    public interface ISalesDbContext : IDbContext
    {
        DbSet<SalesInvoice> SalesInvoices { get; }
    }
}