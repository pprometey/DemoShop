using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoShop.Core.Services
{
    public class PurchaseInvoicesRepository : IDbRepository<PurchaseInvoice>
    {
        private readonly IPurchasesDbContext _db;
        private readonly ShopErrorDescriber _errorDescriber;

        public PurchaseInvoicesRepository(IPurchasesDbContext dbContext, ShopErrorDescriber describer = null)
        {
            this._db = dbContext;
            _errorDescriber = describer ?? new ShopErrorDescriber();
        }

        public async Task<IEnumerable<PurchaseInvoice>> GetAllAsync()
        {
            return await _db.PurchaseInvoices.Include(p => p.Product).ThenInclude(u => u.Unit).AsNoTracking().ToListAsync();
        }

        public async Task<ShopOperationResult> CreateAsync(PurchaseInvoice item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _db.PurchaseInvoices.Add(item);
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

            _db.PurchaseInvoices.Remove(item);
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

        public async Task<ShopOperationResult> UpdateAsync(PurchaseInvoice item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _db.PurchaseInvoices.Attach(item);
            _db.PurchaseInvoices.Update(item);
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

        public async Task<PurchaseInvoice> GetAsync(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await _db.PurchaseInvoices.FindAsync(id);
        }
    }
}