using GestaoAvaliacao.Worker.Rabbit.Consumers;
using GestaoAvaliacao.Worker.Rabbit.Settings;
using GestaoAvaliacao.Worker.StudentTestsSent.Requests.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Consumers
{
    public class StudentTestSentConsumer : GestaoAvaliacaoRabbitBaseConsumer<ProcessStudentTestSentCommand>, IStudentTestSentConsumer
    {
        private readonly IMediator _mediator;

        public StudentTestSentConsumer(IGestaoAvaliacaoRabbitSettings gestaoAvaliacaoRabbitSettings, IMediator mediator)
            : base(gestaoAvaliacaoRabbitSettings)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(CancellationToken cancellationToken)
            => await ConsumeFetchAsync(_mediator.Send, cancellationToken);
    }
}