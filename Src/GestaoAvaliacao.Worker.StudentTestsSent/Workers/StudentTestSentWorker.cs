using GestaoAvaliacao.Worker.StudentTestsSent.Requests.Commands;
using GestaoAvaliacao.Worker.StudentTestsSent.Workers.Scheduling;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Workers
{
    [SchedulingConfig(Cron = "30 2 * * *")]
    public class StudentTestSentWorker : BaseScheduledWorker
    {
        private readonly IMediator _mediator;

        public StudentTestSentWorker(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected override string WorkerDescription => nameof(StudentTestSentWorker);

        protected override Task ExecuteAsync(CancellationToken cancellationToken) => _mediator.Send(new ProcessStudentTestSentCommand());
    }
}