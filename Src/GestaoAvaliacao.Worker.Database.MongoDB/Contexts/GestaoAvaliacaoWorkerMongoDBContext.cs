using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.Attribute;
using MongoDB.Driver;
using System;

namespace GestaoAvaliacao.Worker.Database.MongoDB.Contexts
{
    public class GestaoAvaliacaoWorkerMongoDBContext : IGestaoAvaliacaoWorkerMongoDBContext
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _dataBase;

        public GestaoAvaliacaoWorkerMongoDBContext(string mongoDbConnection, string mongoDbDatabase)
        {
            _client = new MongoClient(mongoDbConnection);
            _dataBase = _client.GetDatabase(mongoDbDatabase);
        }

        public IMongoCollection<T> GetCollection<T>()
            where T : EntityBase
            => _dataBase.GetCollection<T>(GetCollectionName<T>());

        private static string GetCollectionName<T>()
            where T : EntityBase
            => ((CollectionNameAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(CollectionNameAttribute))).Name;
    }
}