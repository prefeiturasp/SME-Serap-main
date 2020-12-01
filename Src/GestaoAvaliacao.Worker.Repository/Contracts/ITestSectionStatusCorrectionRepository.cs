using GestaoAvaliacao.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Contracts
{
    public interface ITestSectionStatusCorrectionRepository
    {
        Task<TestSectionStatusCorrection> GetFirstOrDefaultAsync(long testId, long turId, CancellationToken cancellationToken);
        Task UpdateAsync(TestSectionStatusCorrection entity, CancellationToken cancellationToken);
    }
}