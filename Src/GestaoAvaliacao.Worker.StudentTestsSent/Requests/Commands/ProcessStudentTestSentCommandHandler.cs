using GestaoAvaliacao.Entities.StudentsTestSent;
using GestaoAvaliacao.Worker.Repository.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing.Dtos;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Requests.Commands
{
    internal class ProcessStudentTestSentCommandHandler : AsyncRequestHandler<ProcessStudentTestSentCommand>
    {
        private readonly IStudentTestSentRepository _studentTestSentRepository;
        private readonly IStudentTestSentProcessingChain _studentTestSentProcessingChain;

        public ProcessStudentTestSentCommandHandler(IStudentTestSentRepository studentTestSentRepository, IStudentTestSentProcessingChain studentTestSentProcessingChain)
        {
            _studentTestSentRepository = studentTestSentRepository;
            _studentTestSentProcessingChain = studentTestSentProcessingChain;
        }

        protected override async Task Handle(ProcessStudentTestSentCommand request, CancellationToken cancellationToken)
        {
            var studentTestSent = await GetStudentTestSentToProcessAsync(cancellationToken);
            if (studentTestSent is null) return;

            try
            {
                var dto = new StudentTestSentProcessingChainDto
                {
                    StudentTestSent = studentTestSent
                };

                await _studentTestSentProcessingChain.ExecuteAsync(dto, cancellationToken);
                if (!dto.IsValid)
                {
                    // TO DO LOG
                    await ReturnStudentTestSentToBeProcessedAsync(studentTestSent, cancellationToken);
                    return;
                }

                studentTestSent.SetDone();
                await _studentTestSentRepository.UpdateAsync(studentTestSent, cancellationToken);
            }
            catch (Exception ex)
            {
                // TO DO LOG
                await ReturnStudentTestSentToBeProcessedAsync(studentTestSent, cancellationToken);
            }
        }

        private async Task<StudentTestSent> GetStudentTestSentToProcessAsync(CancellationToken cancellationToken)
        {
            try
            {
                var studentTestSent = await _studentTestSentRepository.GetFirstToProcessAsync(cancellationToken);
                if (studentTestSent is null) return null;

                studentTestSent.SetInProgress();
                if (!studentTestSent.Validate.IsValid)
                {
                    // TO DO LOG
                    return null;
                }

                await _studentTestSentRepository.UpdateAsync(studentTestSent, cancellationToken);
                return studentTestSent;
            }
            catch (Exception ex)
            {
                // TO DO LOG
                return null;
            }
        }

        private async Task ReturnStudentTestSentToBeProcessedAsync(StudentTestSent studentTestSent, CancellationToken cancellationToken)
        {
            if (studentTestSent is null) return;
            studentTestSent.SetPending();
            await _studentTestSentRepository.UpdateAsync(studentTestSent, cancellationToken);
        }
    }
}