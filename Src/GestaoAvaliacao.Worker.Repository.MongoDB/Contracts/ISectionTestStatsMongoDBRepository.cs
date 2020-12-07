using GestaoAvaliacao.MongoEntities;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Contracts
{
    public interface ISectionTestStatsMongoDBRepository
    {
        Task<SectionTestStats> GetClassSectionStatsAsync(long testId, long turId, CancellationToken cancellationToken);
        Task<SectionTestStats> GetEntityAsync(SectionTestStats entity, CancellationToken cancellationToken);
        Task InsertOrReplaceAsync(SectionTestStats entity, CancellationToken cancellationToken);
    }
}