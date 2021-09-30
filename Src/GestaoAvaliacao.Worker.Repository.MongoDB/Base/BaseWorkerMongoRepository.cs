using GestaoAvaliacao.Worker.Database.MongoDB.Contexts;
using GestaoAvaliacao.Worker.Domain.MongoDB.Base;
using GestaoAvaliacao.Worker.Domain.MongoDB.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Base
{
    public abstract class BaseWorkerMongoRepository<T>
         where T : EntityWorkerMongoDBBase, new()
    {
        private readonly IMongoCollection<T> _collection;

        public BaseWorkerMongoRepository(IGestaoAvaliacaoWorkerMongoDBContext gestaoAvaliacaoWorkerMongoDBContext)
        {
            _collection = gestaoAvaliacaoWorkerMongoDBContext.GetCollection<T>();
        }

        public async Task InsertOrReplaceAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity.State == (byte)EnumState.naoDefinido)
            {
                entity.CreateDate = DateTime.Now;
                entity.State = (byte)1;
            }

            entity.UpdateDate = DateTime.Now;
            var filter = Builders<T>.Filter.Eq("_id", entity._id);
            var options = new FindOneAndReplaceOptions<T, T> { IsUpsert = true };
            await _collection.FindOneAndReplaceAsync(filter, entity, options, cancellationToken);
        }

        public async Task<T> ReplaceAsync(T entity, CancellationToken cancellationToken)
        {
            entity.UpdateDate = DateTime.Now;
            var filter = Builders<T>.Filter.Eq("_id", entity._id);
            return await _collection.FindOneAndReplaceAsync(filter, entity, cancellationToken: cancellationToken);
        }

        public async Task<T> GetEntityAsync(T entity, CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.Eq("_id", entity._id);
            var result = _collection.Find<T>(filter).FirstAsync(cancellationToken);

            return await result;
        }

        public async Task<IEnumerable<T>> FindAsync(FilterDefinition<T> filter, CancellationToken cancellationToken)
        {
            return await _collection.Find<T>(filter)?.ToListAsync(cancellationToken);
        }

        public async Task<T> FindOneAsync(FilterDefinition<T> filter, CancellationToken cancellationToken)
        {
            return await _collection.Find<T>(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T> FindOneAsync(T entity, CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.Eq("_id", entity._id);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            entity.UpdateDate = DateTime.Now;
            var filter = Builders<T>.Filter.Eq("_id", entity._id);
            var retorno = await _collection.DeleteOneAsync(filter, cancellationToken);
            return retorno.DeletedCount > 0;
        }
    }
}