using Quartz;
using Quartz.Impl;
using System.Configuration;

namespace GestaoAvaliacao.CopyAnswerSheetFilesService.Quartz
{
	public static class Scheduler
	{
		static IScheduler scheduler;
		public static void Start()
		{
			// INICIALIZA O SCHEDULER
			scheduler = StdSchedulerFactory.GetDefaultScheduler();
			scheduler.Start();

			IJobDetail jobCopy = JobBuilder.Create<CopyAnswerSheetFilesJob>().Build();

			ITrigger triggerCopy = TriggerBuilder.Create()
			.WithIdentity("CopyAnswerSheetFiles", "MSTech")
			.WithCronSchedule(ConfigurationManager.AppSettings["CronCopyAnswerSheetFiles"])
			.StartNow()
			.Build();

			scheduler.ScheduleJob(jobCopy, triggerCopy);

			IJobDetail jobManager = JobBuilder.Create<ManageTestFolderJob>().Build();
			ITrigger triggerManager = TriggerBuilder.Create()
			.WithIdentity("ManageTestFolder", "MSTech")
			.WithCronSchedule(ConfigurationManager.AppSettings["CronTriggerManagerTestFolder"])
			.StartNow()
			.Build();

			scheduler.ScheduleJob(jobManager, triggerManager);
		}

		public static void Stop()
		{
			scheduler.Shutdown(true);
		}
	}
}
