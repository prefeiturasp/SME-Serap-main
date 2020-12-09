using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Workers.Scheduling
{
    public abstract class BaseScheduledWorker : IHostedService
    {
        private const string CronAlwaysRunning = "* * * * *";

        private readonly string _cronWorkerParameterValue;

        protected abstract string WorkerDescription { get; }
        protected abstract string CronWorkerParameter { get; }

        public BaseScheduledWorker(IConfiguration configuration)
        {
            _cronWorkerParameterValue = string.IsNullOrWhiteSpace(CronWorkerParameter) ? null : configuration.GetValue<string>(CronWorkerParameter, default);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var cronSchedule = GetCronParameter();
                var nextOcurrence = cronSchedule.GetNextOccurrence(DateTime.Now);
                var tempoAteProximaExec = nextOcurrence - DateTime.Now;

                Console.WriteLine($"[{WorkerDescription}] Agendado => {nextOcurrence}");
                await Task.Delay(tempoAteProximaExec, cancellationToken);
                await ExecuteAsync(cancellationToken);
                GC.Collect();
            }
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{WorkerDescription}] Parado => {DateTime.Now}");
            return Task.CompletedTask;
        }

        private CrontabSchedule GetCronParameter()
        {
            if (string.IsNullOrWhiteSpace(_cronWorkerParameterValue)) return CrontabSchedule.Parse(CronAlwaysRunning);
            var cron = CrontabSchedule.Parse(_cronWorkerParameterValue, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
            return cron ?? CrontabSchedule.Parse(CronAlwaysRunning);
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}