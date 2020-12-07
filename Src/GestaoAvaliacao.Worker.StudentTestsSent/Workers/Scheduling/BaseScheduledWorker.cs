using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Workers.Scheduling
{
    public abstract class BaseScheduledWorker : IHostedService
    {
        private const string CronAlwaysRunning = "* * * * *";

        protected abstract string WorkerDescription { get; }

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
            var attribute = this.GetType().GetCustomAttribute<SchedulingConfigAttribute>();
            if (attribute is null || string.IsNullOrWhiteSpace(attribute.Cron)) return CrontabSchedule.Parse(CronAlwaysRunning);

            var cron = CrontabSchedule.Parse(attribute.Cron, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
            return cron ?? CrontabSchedule.Parse(CronAlwaysRunning);
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}