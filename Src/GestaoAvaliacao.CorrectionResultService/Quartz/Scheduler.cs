using Quartz;
using Quartz.Impl;
using System.Configuration;

namespace GestaoAvaliacao.CorrectionResultService
{
    public static class Scheduler
	{
		static IScheduler scheduler;
		public static void Start()
		{
			scheduler = StdSchedulerFactory.GetDefaultScheduler();
			scheduler.Start();

			IJobDetail job = JobBuilder.Create<CorrectionResultJob>().Build();

			ITrigger trigger = TriggerBuilder.Create()
			.WithIdentity("GestaoAvaliacaoCorrectionResult", "MSTech")
			.WithCronSchedule(ConfigurationManager.AppSettings["CronTrigger"])
			.StartNow()
			.Build();

			scheduler.ScheduleJob(job, trigger);
		}

		public static void Stop()
		{
			scheduler.Shutdown(true);
		}
	}
}
