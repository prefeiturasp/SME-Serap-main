using Quartz;
using Quartz.Impl;
using System.Configuration;

namespace GestaoAvaliacao.AnswerSheetLotService
{
    public static class Scheduler
	{
		static IScheduler scheduler;
		public static void Start()
		{
			scheduler = StdSchedulerFactory.GetDefaultScheduler();
			scheduler.Start();

			IJobDetail job = JobBuilder.Create<AnswerSheetLotJob>().Build();

			ITrigger trigger = TriggerBuilder.Create()
			.WithIdentity("GestaoAvaliacaoAnswerSheetLot", "MSTech")
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
