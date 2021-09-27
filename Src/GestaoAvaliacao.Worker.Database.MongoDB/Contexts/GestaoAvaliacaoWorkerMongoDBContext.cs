using GestaoAvaliacao.Worker.Database.MongoDB.Settings;
using GestaoAvaliacao.Worker.Domain.MongoDB.Base;
using GestaoAvaliacao.Worker.Domain.MongoDB.Base.Attributes;
using MongoDB.Driver;
using System;

namespace GestaoAvaliacao.Worker.Database.MongoDB.Contexts
{
    public class GestaoAvaliacaoWorkerMongoDBContext : IGestaoAvaliacaoWorkerMongoDBContext
    {
        private IMongoClient _client;
        private IMongoDatabase _dataBase;
        private readonly IGestaoAvaliacaoWorkerMongoDBSettings _gestaoAvaliacaoWorkerMongoDBSettings;

        public GestaoAvaliacaoWorkerMongoDBContext(IGestaoAvaliacaoWorkerMongoDBSettings gestaoAvaliacaoWorkerMongoDBSettings)
        {
            _gestaoAvaliacaoWorkerMongoDBSettings = gestaoAvaliacaoWorkerMongoDBSettings;
        }

        public IMongoCollection<T> GetCollection<T>()
            where T : EntityWorkerMongoDBBase
            => Database.GetCollection<T>(GetCollectionName<T>());

        private IMongoClient Client 
        { 
            get
            {
                if(_client is null)
                {
                    var mongoClientSettings = MongoClientSettings.FromConnectionString(_gestaoAvaliacaoWorkerMongoDBSettings.ConnectionString);
                    mongoClientSettings.RetryReads = true;
                    mongoClientSettings.RetryWrites = true;
                    mongoClientSettings.ConnectTimeout = TimeSpan.FromMinutes(_gestaoAvaliacaoWorkerMongoDBSettings.ConnectTimeoutInMinutes);
                    _client = new MongoClient(mongoClientSettings);
                }

                return _client;
            }
        }

        private IMongoDatabase Database
        {
            get
            {
                if(_dataBase is null)
                {
                    _dataBase = Client.GetDatabase(_gestaoAvaliacaoWorkerMongoDBSettings.Database);
                }

                return _dataBase;
            }
        }

        private static string GetCollectionName<T>()
            where T : EntityWorkerMongoDBBase
            => ((CollectionNameWorkerAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(CollectionNameWorkerAttribute))).Name;
    }
}