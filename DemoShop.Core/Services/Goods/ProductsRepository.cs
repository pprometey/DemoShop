using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoShop.Core.Services
{
    public class ProductsRepository : IDbRepository<Product>
    {
        private readonly IGoodsDbContext _db;
        private readonly ShopErrorDescriber _errorDescriber;

        public ProductsRepository(IGoodsDbContext dbContext, ShopErrorDescriber describer = null)
        {
            this._db = dbContext;
            _errorDescriber = describer ?? new ShopErrorDescriber();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _db.Products.Include(p => p.ProductsCategory).Include(u => u.Unit).AsNoTracking().ToListAsync();
        }

        public async Task<ShopOperationResult> CreateAsync(Product item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _db.Products.Add(item);
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

            _db.Products.Remove(item);
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

        public async Task<ShopOperationResult> UpdateAsync(Product item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _db.Products.Attach(item);
            _db.Products.Update(item);
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

        public async Task<Product> GetAsync(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await _db.Products.FindAsync(id);
        }
    }
}