using GestaoAvaliacao.Worker.StudentTestsSent.Consumers;
using GestaoAvaliacao.Worker.StudentTestsSent.Logging;
using GestaoAvaliacao.Worker.StudentTestsSent.Workers.Scheduling;
using Microsoft.Extensions.Configuration;
using Prometheus.DotNetRuntime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Workers
{
    public class StudentTestSentWorker : BaseScheduledWorker
    {
        private readonly IStudentTestSentConsumer _studentTestSentConsummer;
        private IDisposable _collector;

        public StudentTestSentWorker(IConfiguration configuration, ISentryLogger sentryLogger, IStudentTestSentConsumer studentTestSentConsummer)
            : base(configuration, sentryLogger)
        {
            _studentTestSentConsummer = studentTestSentConsummer;
        }

        protected override string WorkerDescription => nameof(StudentTestSentWorker);

        protected override string CronWorkerParameter => $"{nameof(StudentTestSentWorker)}_CronParameter";

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();
            await _studentTestSentConsummer.ConsumeAsync(cancellationToken);
        }

        protected override Task OnStoppingAsync()
        {
            _studentTestSentConsummer.Close();
            return base.OnStoppingAsync();
        }
    }
}