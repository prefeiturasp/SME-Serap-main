using GestaoAvaliacao.Worker.Database.MongoDB.Contexts;
using GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests;
using GestaoAvaliacao.Worker.Repository.MongoDB.Base;
using GestaoAvaliacao.Worker.Repository.MongoDB.Contracts;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Tests
{
    public class CorrectionResultsMongoDBRepository : BaseWorkerMongoRepository<CorrectionResultsEntityWorker>, ICorrectionResultsMongoDBRepository
    {
        public CorrectionResultsMongoDBRepository(IGestaoAvaliacaoWorkerMongoDBContext gestaoAvaliacaoWorkerMongoDBContext)
            : base(gestaoAvaliacaoWorkerMongoDBContext)
        {
        }
    }
}