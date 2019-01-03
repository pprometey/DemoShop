using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoShop.Core.Services
{
    public class ProductsCategoriesRepository : IDbRepository<ProductsCategory>
    {
        private readonly IGoodsDbContext _db;
        private readonly ShopErrorDescriber _errorDescriber;

        public ProductsCategoriesRepository(IGoodsDbContext dbContext, ShopErrorDescriber describer = null)
        {
            this._db = dbContext;
            _errorDescriber = describer ?? new ShopErrorDescriber();
        }

        public async Task<IEnumerable<ProductsCategory>> GetAllAsync()
        {
            return await _db.ProductsCategories.ToListAsync();
        }

        public async Task<ShopOperationResult> CreateAsync(ProductsCategory item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _db.ProductsCategories.Add(item);
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

            _db.ProductsCategories.Remove(item);
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

        public async Task<ShopOperationResult> UpdateAsync(ProductsCategory item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _db.ProductsCategories.Attach(item);
            _db.ProductsCategories.Update(item);
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

        public async Task<ProductsCategory> GetAsync(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await _db.ProductsCategories.FindAsync(id);
        }
    }
}