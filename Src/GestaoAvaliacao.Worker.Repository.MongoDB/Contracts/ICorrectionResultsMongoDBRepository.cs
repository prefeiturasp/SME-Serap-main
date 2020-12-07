using GestaoAvaliacao.MongoEntities;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Contracts
{
    public interface ICorrectionResultsMongoDBRepository
    {
        Task InsertOrReplaceAsync(CorrectionResults entity, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(CorrectionResults entity, CancellationToken cancellationToken);
    }
}