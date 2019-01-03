using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoShop.Core.Services
{
    public class SalesInvoicesRepository : ISalesInvoceDbRepository<SalesInvoice>
    {
        private readonly ISalesDbContext _db;
        private readonly ShopErrorDescriber _errorDescriber;

        public SalesInvoicesRepository(ISalesDbContext dbContext, ShopErrorDescriber describer = null)
        {
            this._db = dbContext;
            _errorDescriber = describer ?? new ShopErrorDescriber();
        }

        public async Task<IEnumerable<SalesInvoice>> GetAllAsync()
        {
            return await _db.SalesInvoices.Include(p => p.Product).ThenInclude(u => u.Unit).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<SalesInvoice>> GetByDateAsync(DateTime date)
        {
            if (date == null)
            {
                throw new ArgumentNullException(nameof(date));
            }

            return await _db.SalesInvoices.Include(p => p.Product).ThenInclude(u => u.Unit).Where(d => (d.SalesDate.Date == date.Date)).AsNoTracking().ToListAsync();
        }

        public async Task<ShopOperationResult> CreateAsync(SalesInvoice item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _db.SalesInvoices.Add(item);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch
            {
                return ShopOperationResult.Failed(_errorDescriber.DefaultError());
            }
            return ShopOperationResult.Success;
        }

        public async Task<ShopOperationResult> DeleteAsync(Guid id)
        {
            var item = await GetAsync(id);

            if (item == null)
            {
                return ShopOperationResult.Failed(_errorDescriber.NotFoundItem());
            }

            _db.SalesInvoices.Remove(item);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ShopOperationResult.Failed(_errorDescriber.ConcurrencyFailure());
            }
            return ShopOperationResult.Success;
        }

        public async Task<ShopOperationResult> UpdateAsync(SalesInvoice item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _db.SalesInvoices.Attach(item);
            _db.SalesInvoices.Update(item);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ShopOperationResult.Failed(_errorDescriber.ConcurrencyFailure());
            }
            return ShopOperationResult.Success;
        }

        public async Task<SalesInvoice> GetAsync(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await _db.SalesInvoices.FindAsync(id);
        }

        public Task<bool> CheckBalance(Product product)
        {
            throw new NotImplementedException();
        }
    }
}