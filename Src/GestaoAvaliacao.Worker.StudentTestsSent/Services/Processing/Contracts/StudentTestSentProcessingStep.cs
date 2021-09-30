using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Processing.Contracts
{
    internal abstract class StudentTestSentProcessingStep : IStudentTestSentProcessingStep
    {
        private IStudentTestSentProcessingStep _nextStep;

        public async Task ExecuteAsync(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken)
        {
            if (!dto?.IsValid ?? true) return;
            await OnExecuting(dto, cancellationToken);
            if (_nextStep != null) await _nextStep.ExecuteAsync(dto, cancellationToken);
        }

        protected abstract Task OnExecuting(StudentTestSentProcessingChainDto dto, CancellationToken cancellationToken);

        public void SetNextStep(IStudentTestSentProcessingStep nextStep) => _nextStep = nextStep;
    }
}