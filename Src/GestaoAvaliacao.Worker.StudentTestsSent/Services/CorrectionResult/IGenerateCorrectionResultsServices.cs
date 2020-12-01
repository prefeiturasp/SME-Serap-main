using GestaoAvaliacao.Worker.StudentTestsSent.Services.CorrectionResult.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Services.CorrectionResult
{
    internal interface IGenerateCorrectionResultsServices
    {
        Task ExecuteAsync(GenerateCorrectionResultsDto dto, CancellationToken cancellationToken);
    }
}