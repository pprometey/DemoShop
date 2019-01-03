using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoShop.Core.Infrastructure
{
    public interface IDbRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetAsync(Guid id);

        Task<ShopOperationResult> CreateAsync(T item);

        Task<ShopOperationResult> UpdateAsync(T item);

        Task<ShopOperationResult> DeleteAsync(Guid id);
    }
}