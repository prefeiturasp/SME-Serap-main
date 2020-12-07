using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.Attribute;
using GestaoAvaliacao.Worker.Database.MongoDB.Settings;
using MongoDB.Driver;
using System;

namespace GestaoAvaliacao.Worker.Database.MongoDB.Contexts
{
    public class GestaoAvaliacaoWorkerMongoDBContext : IGestaoAvaliacaoWorkerMongoDBContext
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _dataBase;

        public GestaoAvaliacaoWorkerMongoDBContext(IGestaoAvaliacaoWorkerMongoDBSettings gestaoAvaliacaoWorkerMongoDSettings)
        {
            _client = new MongoClient(gestaoAvaliacaoWorkerMongoDSettings.ConnectionString);
            _dataBase = _client.GetDatabase(gestaoAvaliacaoWorkerMongoDSettings.Database);
        }

        public IMongoCollection<T> GetCollection<T>()
            where T : EntityBase
            => _dataBase.GetCollection<T>(GetCollectionName<T>());

        private static string GetCollectionName<T>()
            where T : EntityBase
            => ((CollectionNameAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(CollectionNameAttribute))).Name;
    }
}