using GestaoAvaliacao.Worker.StudentTestsSent.Requests.Commands;
using GestaoAvaliacao.Worker.StudentTestsSent.Workers.Scheduling;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Workers
{
    public class StudentTestSentWorker : BaseScheduledWorker
    {
        private readonly IMediator _mediator;

        public StudentTestSentWorker(IMediator mediator, IConfiguration configuration)
            :base(configuration)
        {
            _mediator = mediator;
        }

        protected override string WorkerDescription => nameof(StudentTestSentWorker);

        protected override string CronWorkerParameter => $"{nameof(StudentTestSentWorker)}_CronParameter";

        protected override Task ExecuteAsync(CancellationToken cancellationToken) => _mediator.Send(new ProcessStudentTestSentCommand());
    }
}