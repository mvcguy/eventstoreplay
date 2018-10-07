using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using expense.web.api.Values.ReadModel.Schema;
using MongoDB.Driver;

namespace expense.web.api.Values.ReadModel
{
    public interface IReadModelRepository<TEntity> where TEntity : class, IEntityBase, new()
    {
        IMongoCollection<TEntity> Collection { get; }

        Task<bool> AddOrUpdateAsync(TEntity entity, IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> UpdateAsync(TEntity entity, IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken));

        Task<TEntity> AddAsync(TEntity entity, IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken));

        Task AddManyAsync(IEnumerable<TEntity> entities, IClientSessionHandle session,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default(CancellationToken));

        IQueryable<TEntity> GetAll();

        Task<bool> RemoveByIdAsync(object id, IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> RemoveEntityAsync(TEntity entity, IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken));

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}