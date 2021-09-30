using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Processing.Contracts
{
    internal interface IStudentTestSentProcessingStep
    {
        Task ExecuteAsync(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken);

        void SetNextStep(IStudentTestSentProcessingStep nextStep);
    }
}