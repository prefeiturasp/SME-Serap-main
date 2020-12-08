using GestaoAvaliacao.Worker.Domain.Entities.Tests;
using GestaoAvaliacao.Worker.Repository.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Logging;
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
            do
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
                        SentryLogger.LogErrors(dto.Errors);
                        await ReturnStudentTestSentToBeProcessedAsync(studentTestSent, cancellationToken);
                        return;
                    }

                    studentTestSent.SetDone();
                    await _studentTestSentRepository.UpdateAsync(studentTestSent, cancellationToken);
                }
                catch (Exception ex)
                {
                    SentryLogger.LogError(ex);
                    await ReturnStudentTestSentToBeProcessedAsync(studentTestSent, cancellationToken);
                }
            } while (!cancellationToken.IsCancellationRequested);
        }

        private async Task<StudentTestSentEntityWorker> GetStudentTestSentToProcessAsync(CancellationToken cancellationToken)
        {
            try
            {
                var studentTestSent = await _studentTestSentRepository.GetFirstToProcessAsync(cancellationToken);
                if (studentTestSent is null) return null;

                studentTestSent.SetInProgress();
                if (!studentTestSent.IsValid)
                {
                    SentryLogger.LogErrors(studentTestSent.Errors);
                    return null;
                }

                await _studentTestSentRepository.UpdateAsync(studentTestSent, cancellationToken);
                return studentTestSent;
            }
            catch (Exception ex)
            {
                SentryLogger.LogError(ex);
                return null;
            }
        }

        private async Task ReturnStudentTestSentToBeProcessedAsync(StudentTestSentEntityWorker studentTestSent, CancellationToken cancellationToken)
        {
            if (studentTestSent is null) return;
            studentTestSent.SetPending();
            await _studentTestSentRepository.UpdateAsync(studentTestSent, cancellationToken);
        }
    }
}