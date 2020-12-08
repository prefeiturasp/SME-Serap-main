using GestaoAvaliacao.Worker.Database.MongoDB.Contexts;
using GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests;
using GestaoAvaliacao.Worker.Repository.MongoDB.Base;
using GestaoAvaliacao.Worker.Repository.MongoDB.Contracts;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Tests
{
    public class TestTemplateMongoDBRepository : BaseWorkerMongoRepository<TestTemplateEntityWorker>, ITestTemplateMongoDBRepository
    {
        public TestTemplateMongoDBRepository(IGestaoAvaliacaoWorkerMongoDBContext gestaoAvaliacaoWorkerMongoDBContext)
            : base(gestaoAvaliacaoWorkerMongoDBContext)
        {
        }
    }
}