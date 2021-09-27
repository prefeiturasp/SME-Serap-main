using Quartz;
using Quartz.Impl;
using System.Configuration;

namespace GestaoAvaliacao.ExportAnalysisService.Quartz
{
    public static class Scheduler
	{
		static IScheduler scheduler;
		public static void Start()
		{
			// INICIALIZA O SCHEDULER
			scheduler = StdSchedulerFactory.GetDefaultScheduler();
			scheduler.Start();

			#region [ Import SGP ]
			IJobDetail job = JobBuilder
				.Create<ExportAnalysisJob>()
				.RequestRecovery(false)
				.Build();

			ITrigger trigger = TriggerBuilder.Create()
				.WithIdentity("GestaoAvaliacaoExportAnalysis", "MSTech")
				.WithCronSchedule(ConfigurationManager.AppSettings["CronTrigger"])
				.StartNow()
				.Build();

			scheduler.ScheduleJob(job, trigger);
			#endregion
		}

		public static void Stop()
		{
			scheduler.Shutdown(true);
		}
	}
}
