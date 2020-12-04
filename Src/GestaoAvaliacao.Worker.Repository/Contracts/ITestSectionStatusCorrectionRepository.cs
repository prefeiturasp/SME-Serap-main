using GestaoAvaliacao.Worker.Domain.Entities.Tests;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Contracts
{
    public interface ITestSectionStatusCorrectionRepository
    {
        Task<TestSectionStatusCorrectionEntityWorker> GetFirstOrDefaultAsync(long testId, long turId, CancellationToken cancellationToken);

        Task UpdateAsync(TestSectionStatusCorrectionEntityWorker entity, CancellationToken cancellationToken);
    }
}