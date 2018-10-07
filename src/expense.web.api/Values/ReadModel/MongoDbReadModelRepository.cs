using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using expense.web.api.Values.ReadModel.Schema;
using MongoDB.Driver;

namespace expense.web.api.Values.ReadModel
{
    public class MongoDbReadModelRepository<TEntity> : IReadModelRepository<TEntity> where TEntity : class, IEntityBase, new()
    {
        public IMongoCollection<TEntity> Collection { get; }

        public IMongoDatabase Database { get; }

        public MongoDbReadModelRepository(IMongoDatabase database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));

            Collection = database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public async Task<bool> AddOrUpdateAsync(TEntity entity, IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var result = await Collection
                .ReplaceOneAsync(p => p.Id == entity.Id, entity,
                    new UpdateOptions
                    {
                        IsUpsert = true
                    }, cancellationToken: cancellationToken);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateAsync(TEntity entity, IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var doc = await this.GetByIdAsync(entity.Id, cancellationToken);
            if (doc == null) return false;

            var result = await Collection
                .ReplaceOneAsync(session, p => p.Id == entity.Id, entity,
                    new UpdateOptions
                    {
                        IsUpsert = true
                    }, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<TEntity> AddAsync(TEntity entity, IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await Collection.InsertOneAsync(session, entity, cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }

        public async Task AddManyAsync(IEnumerable<TEntity> entities, IClientSessionHandle session, CancellationToken cancellationToken = new CancellationToken())
        {

            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            cancellationToken.ThrowIfCancellationRequested();
            await Collection.InsertManyAsync(session, entities, cancellationToken: cancellationToken);
        }

        public async Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);
            var doc = await Collection.Find(filter).ToListAsync(cancellationToken: cancellationToken);
            return doc.FirstOrDefault();
        }

        public async Task<TEntity> GetByAggregateId(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var filter = Builders<TEntity>.Filter.Eq(x => x.PublicId, id);
            var doc = await Collection.Find(filter).ToListAsync(cancellationToken: cancellationToken);
            return doc.FirstOrDefault();
        }

        public IQueryable<TEntity> GetAll()
        {
            return Collection.AsQueryable();
        }

        public async Task<bool> RemoveByIdAsync(object id, IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);
            var result = await Collection.DeleteOneAsync(session, filter, cancellationToken: cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task<bool> RemoveEntityAsync(TEntity entity, IClientSessionHandle session, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await this.RemoveByIdAsync(entity.Id, session, cancellationToken);
        }

    }
}