using DemoShop.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DemoShop.Core.Infrastructure
{
    public interface ISalesInvoceDbRepository<T> : IDbRepository<T> where T : class
    {
        Task<bool> CheckBalance(Product product);

        Task<IEnumerable<T>> GetByDateAsync(DateTime date);
    }
}