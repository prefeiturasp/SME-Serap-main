using Castle.Windsor;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.Services;
using Quartz;
using System;
using System.Threading.Tasks;

namespace GestaoAvaliacao.ExportAnalysisService.Quartz
{
    public class ExportAnalysisJob : IJob
	{
		static Task _singleTask;

		public void Execute(IJobExecutionContext context)
		{
			if (_singleTask != null && !_singleTask.IsCompleted && !_singleTask.IsFaulted && !_singleTask.IsCanceled)
				return;

            IWindsorContainer container = new WindsorContainer()
                .Install(new BusinessInstaller() { LifestylePerWebRequest = false })
				.Install(new RepositoriesInstaller() { LifestylePerWebRequest = false })
				.Install(new StorageInstaller() { LifestylePerWebRequest = false })
				.Install(new PDFConverterInstaller() { LifestylePerWebRequest = false })
				.Install(new ServiceContainerInstaller());

			var service = container.Resolve<GestaoAvaliacao.Services.ExportAnalysisService>();

			_singleTask = Task.Run(() => service.Execute());
			_singleTask.Wait();
		}
	}
}
