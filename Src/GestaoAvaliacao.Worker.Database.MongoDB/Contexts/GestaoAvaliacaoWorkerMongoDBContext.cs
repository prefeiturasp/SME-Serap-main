using GestaoAvaliacao.Worker.Database.MongoDB.Settings;
using GestaoAvaliacao.Worker.Domain.MongoDB.Base;
using GestaoAvaliacao.Worker.Domain.MongoDB.Base.Attributes;
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
            where T : EntityWorkerMongoDBBase
            => _dataBase.GetCollection<T>(GetCollectionName<T>());

        private static string GetCollectionName<T>()
            where T : EntityWorkerMongoDBBase
            => ((CollectionNameWorkerAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(CollectionNameWorkerAttribute))).Name;
    }
}