using GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Contracts
{
    public interface ISectionTestStatsMongoDBRepository
    {
        Task<SectionTestStatsEntityWorker> GetClassSectionStatsAsync(long testId, long turId, CancellationToken cancellationToken);

        Task<SectionTestStatsEntityWorker> GetEntityAsync(SectionTestStatsEntityWorker entity, CancellationToken cancellationToken);

        Task InsertOrReplaceAsync(SectionTestStatsEntityWorker entity, CancellationToken cancellationToken);
    }
}