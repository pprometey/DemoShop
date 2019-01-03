using System.Threading;
using System.Threading.Tasks;

namespace DemoShop.Core.Infrastructure
{
    //TODO: рефакторинг (убрать лишние)
    public interface IDbContext
    {
        int SaveChanges();

        int SaveChanges(bool acceptAllChangesOnSuccess);

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}