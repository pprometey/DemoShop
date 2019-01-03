using DemoShop.Core.Domain;

namespace DemoShop.Core.Infrastructure
{
    public interface ISalesDbRepositories
    {
        ISalesInvoceDbRepository<SalesInvoice> SalesInvoices { get; }
    }
}