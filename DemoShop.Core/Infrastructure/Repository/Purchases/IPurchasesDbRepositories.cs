using DemoShop.Core.Domain;

namespace DemoShop.Core.Infrastructure
{
    public interface IPurchasesDbRepositories
    {
        IDbRepository<PurchaseInvoice> PurchaseInvoices { get; }
    }
}