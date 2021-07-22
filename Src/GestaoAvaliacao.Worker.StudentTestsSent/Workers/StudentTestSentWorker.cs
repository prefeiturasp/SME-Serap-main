using GestaoAvaliacao.Worker.StudentTestsSent.Consumers;
using GestaoAvaliacao.Worker.StudentTestsSent.Logging;
using Microsoft.Extensions.Hosting;
using MediatR;
using Microsoft.Extensions.Configuration;
using Prometheus.DotNetRuntime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Workers
{
    public class StudentTestSentWorker : IHostedService
    {
        private readonly IMediator _mediator;
        private IDisposable _collector;

        private IStudentTestSentConsumer _studentTestSentConsumer;

        public StudentTestSentWorker(IConfiguration configuration, ISentryLogger sentryLogger, IMediator mediator, IStudentTestSentConsumer studentTestSentConsumer)
        {
            _mediator = mediator;
            _studentTestSentConsumer = studentTestSentConsumer;
            _collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _studentTestSentConsumer.ConsumeAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => _collector.Dispose());
        }
    }
}