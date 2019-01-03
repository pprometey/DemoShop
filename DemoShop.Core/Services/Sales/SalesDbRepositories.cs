using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;

namespace DemoShop.Core.Services
{
    public class SalesDbRepositories : ISalesDbRepositories
    {
        private readonly ISalesDbContext _context;

        private ISalesInvoceDbRepository<SalesInvoice> _salesInvoices;

        public SalesDbRepositories(ISalesDbContext context)
        {
            _context = context;
        }

        public ISalesInvoceDbRepository<SalesInvoice> SalesInvoices
        {
            get
            {
                if (_salesInvoices == null)
                    _salesInvoices = new SalesInvoicesRepository(_context);
                return _salesInvoices;
            }
        }
    }
}