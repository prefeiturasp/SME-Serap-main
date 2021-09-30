using GestaoAvaliacao.Worker.Domain.MongoDB.Entities.Tests;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.Contracts
{
    public interface ICorrectionResultsMongoDBRepository
    {
        Task InsertOrReplaceAsync(CorrectionResultsEntityWorker entity, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(CorrectionResultsEntityWorker entity, CancellationToken cancellationToken);
    }
}