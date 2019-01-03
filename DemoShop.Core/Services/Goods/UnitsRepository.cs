using DemoShop.Core.Domain;
using DemoShop.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoShop.Core.Services
{
    public class UnitsRepository : IDbRepository<Unit>
    {
        private readonly IGoodsDbContext _db;
        private readonly ShopErrorDescriber _errorDescriber;

        public UnitsRepository(IGoodsDbContext dbContext, ShopErrorDescriber describer = null)
        {
            this._db = dbContext;
            _errorDescriber = describer ?? new ShopErrorDescriber();
        }

        public async Task<IEnumerable<Unit>> GetAllAsync()
        {
            return await _db.Units.AsNoTracking().ToListAsync();
        }

        public async Task<ShopOperationResult> CreateAsync(Unit item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _db.Units.Add(item);
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

            _db.Units.Remove(item);
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

        public async Task<ShopOperationResult> UpdateAsync(Unit item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            _db.Units.Attach(item);
            _db.Units.Update(item);
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

        public async Task<Unit> GetAsync(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await _db.Units.FindAsync(id);
        }
    }
}