using GestaoAvaliacao.MongoEntities;
using MongoDB.Driver;

namespace GestaoAvaliacao.Worker.Database.MongoDB.Contexts
{
    public interface IGestaoAvaliacaoWorkerMongoDBContext
    {
        IMongoCollection<T> GetCollection<T>() where T : EntityBase;
    }
}