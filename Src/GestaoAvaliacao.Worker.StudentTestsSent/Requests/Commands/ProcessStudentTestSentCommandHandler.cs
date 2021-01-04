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
        private readonly ISentryLogger _sentryLogger;

        public ProcessStudentTestSentCommandHandler(IStudentTestSentRepository studentTestSentRepository, IStudentTestSentProcessingChain studentTestSentProcessingChain,
            ISentryLogger sentryLogger)
        {
            _studentTestSentRepository = studentTestSentRepository;
            _studentTestSentProcessingChain = studentTestSentProcessingChain;
            _sentryLogger = sentryLogger;
        }

        protected override async Task Handle(ProcessStudentTestSentCommand request, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var studentTestSent = await _studentTestSentRepository.GetFirstPendingAsync(cancellationToken);
                    if (studentTestSent is null) return;

                    studentTestSent.SetInProgress();
                    if (!studentTestSent.IsValid)
                    {
                        _sentryLogger.LogErrors(studentTestSent.Errors);
                        return;
                    }

                    await _studentTestSentRepository.UpdateAsync(studentTestSent, cancellationToken);
                    await ProcessAsync(studentTestSent, cancellationToken);
                }
                catch (Exception ex)
                {
                    _sentryLogger.LogError(ex);
                }
            }
        }

        private async Task ProcessAsync(StudentTestSentEntityWorker studentTestSent, CancellationToken cancellationToken)
        {
            try
            {
                var dto = new StudentTestSentProcessingChainDto
                {
                    StudentTestSent = studentTestSent
                };

                await _studentTestSentProcessingChain.ExecuteAsync(dto, cancellationToken);
                if (!dto.IsValid)
                {
                    _sentryLogger.LogErrors(dto.Errors);
                    await UpdateStudentTestSentSituationToWarningAsync(studentTestSent, cancellationToken);
                    return;
                }

                studentTestSent.SetDone();
                await _studentTestSentRepository.UpdateAsync(studentTestSent, cancellationToken);
            }
            catch (Exception ex)
            {
                _sentryLogger.LogError(ex);
                await UpdateStudentTestSentSituationToWarningAsync(studentTestSent, cancellationToken);
            }
        }

        private async Task UpdateStudentTestSentSituationToWarningAsync(StudentTestSentEntityWorker studentTestSent, CancellationToken cancellationToken)
        {
            if (studentTestSent is null) return;
            studentTestSent.SetWarning();
            await _studentTestSentRepository.UpdateAsync(studentTestSent, cancellationToken);
        }
    }
}