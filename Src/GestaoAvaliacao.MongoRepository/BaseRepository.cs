using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.Attribute;
using MongoDB.Bson;
using MongoDB.Driver;
using MSTech.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace GestaoAvaliacao.MongoRepository
{
    public abstract class BaseRepository<T>
         where T : EntityBase, new()
    {
        #region Propriedades

        private IMongoClient _client;
        private IMongoDatabase _dataBase;

        public IMongoClient Client
        {
            get
            {
                if (_client == null)
                {
                    SymmetricAlgorithm cripto = new SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES);
                    string MongoDB_Connection = ConfigurationManager.AppSettings["MongoDB_Connection"];
                    BsonDefaults.GuidRepresentation = GuidRepresentation.CSharpLegacy;

                    _client = new MongoClient(cripto.Decrypt(MongoDB_Connection));
                }

                return _client;
            }
        }

        protected IMongoDatabase DataBase
        {
            get
            {
                if (_dataBase == null)
                    _dataBase = Client.GetDatabase(ConfigurationManager.AppSettings["MongoDB_Database"]);

                return _dataBase;
            }
        }

        public IMongoCollection<T> Collection
        {
            get
            {
                return DataBase.GetCollection<T>(CollectionName);
            }
        }

        public string CollectionName
        {
            get
            {
                return ((CollectionNameAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(CollectionNameAttribute))).Name;
            }
        }

        #endregion Propriedades

        public async Task<T> Insert(T entity)
        {
            entity.CreateDate = DateTime.Now;
            entity.UpdateDate = DateTime.Now;
            entity.State = (byte)1;
            await Collection.InsertOneAsync(entity);

            return entity;
        }

        public void InsertMany(List<T> entity)
        {
            Collection.InsertMany(entity);
        }

        public async Task InsertManyAsync(List<T> entity) => await Collection.InsertManyAsync(entity);

        public async Task InsertOrReplaceAsync(T entity)
        {
            if(entity.State == (byte)EnumState.naoDefinido)
            {
                entity.CreateDate = DateTime.Now;
                entity.State = (byte)1;
            }

            entity.UpdateDate = DateTime.Now;
            var filter = Builders<T>.Filter.Eq("_id", entity._id);
            var options = new FindOneAndReplaceOptions<T, T> { IsUpsert = true };
            await Collection.FindOneAndReplaceAsync(filter, entity, options);
        }

        public async Task<T> Replace(T entity)
        {
            entity.UpdateDate = DateTime.Now;
            var filter = Builders<T>.Filter.Eq("_id", entity._id);
            return await Collection.FindOneAndReplaceAsync(filter, entity);
        }

        public async Task<T> GetEntity(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", entity._id);
            var result = Collection.Find<T>(filter).FirstAsync();

            return await result;
        }

        public async Task<long> Count(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", entity._id);
            return await Collection.CountAsync(filter);
        }

        public async Task<long> Count(FilterDefinition<T> filter)
        {
            return await Collection.Find<T>(filter).CountAsync();
        }

        public async Task<List<T>> Find(FilterDefinition<T> filter)
        {
            return await Collection.Find<T>(filter)?.ToListAsync();
        }

        public async Task<T> FindOne(FilterDefinition<T> filter)
        {
            return await Collection.Find<T>(filter).FirstOrDefaultAsync();
        }

        public async Task<T> FindOneAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", entity._id);
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> Delete(T entity)
        {
            entity.UpdateDate = DateTime.Now;
            var filter = Builders<T>.Filter.Eq("_id", entity._id);
            var retorno = await Collection.DeleteOneAsync(filter);
            return retorno.DeletedCount > 0;
        }
    }
}