using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Processing
{
    internal interface IStudentTestSentProcessingChain
    {
        Task ExecuteAsync(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken);
    }
}