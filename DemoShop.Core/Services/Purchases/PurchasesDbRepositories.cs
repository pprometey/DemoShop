using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;

namespace DemoShop.Core.Services
{
    public class PurchasesDbRepositories : IPurchasesDbRepositories
    {
        private readonly IPurchasesDbContext _context;

        private IDbRepository<PurchaseInvoice> _purchaseInvoices;

        public PurchasesDbRepositories(IPurchasesDbContext context)
        {
            _context = context;
        }

        public IDbRepository<PurchaseInvoice> PurchaseInvoices
        {
            get
            {
                if (_purchaseInvoices == null)
                    _purchaseInvoices = new PurchaseInvoicesRepository(_context);
                return _purchaseInvoices;
            }
        }
    }
}