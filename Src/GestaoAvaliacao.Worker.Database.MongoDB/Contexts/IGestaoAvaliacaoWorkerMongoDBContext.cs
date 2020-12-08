using GestaoAvaliacao.Worker.Domain.MongoDB.Base;
using MongoDB.Driver;

namespace GestaoAvaliacao.Worker.Database.MongoDB.Contexts
{
    public interface IGestaoAvaliacaoWorkerMongoDBContext
    {
        IMongoCollection<T> GetCollection<T>() where T : EntityWorkerMongoDBBase;
    }
}