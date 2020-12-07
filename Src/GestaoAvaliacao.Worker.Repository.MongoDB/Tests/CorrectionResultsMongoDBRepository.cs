using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.Worker.Database.MongoDB.Contexts;
using GestaoAvaliacao.Worker.Repository.MongoDB.Base;
using GestaoAvaliacao.Worker.Repository.MongoDB.Contracts;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Tests
{
    public class CorrectionResultsMongoDBRepository : BaseWorkerMongoRepository<CorrectionResults>, ICorrectionResultsMongoDBRepository
    {
        public CorrectionResultsMongoDBRepository(IGestaoAvaliacaoWorkerMongoDBContext gestaoAvaliacaoWorkerMongoDBContext)
            : base(gestaoAvaliacaoWorkerMongoDBContext)
        {
        }
    }
}