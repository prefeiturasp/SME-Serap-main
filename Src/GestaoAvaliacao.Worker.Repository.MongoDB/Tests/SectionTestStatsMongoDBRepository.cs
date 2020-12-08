using GestaoAvaliacao.Worker.Database.MongoDB.Contexts;
using GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests;
using GestaoAvaliacao.Worker.Repository.MongoDB.Base;
using GestaoAvaliacao.Worker.Repository.MongoDB.Contracts;
using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Tests
{
    public class SectionTestStatsMongoDBRepository : BaseWorkerMongoRepository<SectionTestStatsEntityWorker>, ISectionTestStatsMongoDBRepository
    {
        public SectionTestStatsMongoDBRepository(IGestaoAvaliacaoWorkerMongoDBContext gestaoAvaliacaoWorkerMongoDBContext)
            : base(gestaoAvaliacaoWorkerMongoDBContext)
        {
        }

        public Task<SectionTestStatsEntityWorker> GetClassSectionStatsAsync(long testId, long turId, CancellationToken cancellationToken)
        {
            var filter1 = Builders<SectionTestStatsEntityWorker>.Filter.Eq("Test_Id", testId);
            var filter2 = Builders<SectionTestStatsEntityWorker>.Filter.Eq("tur_id", turId);
            var filter = Builders<SectionTestStatsEntityWorker>.Filter.And(filter1, filter2);
            return FindOneAsync(filter, cancellationToken);
        }
    }
}