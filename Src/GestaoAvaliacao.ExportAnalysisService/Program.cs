using Castle.Windsor;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.Services;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace GestaoAvaliacao.ExportAnalysisService
{
    static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			IWindsorContainer container = new WindsorContainer()
				.Install(new BusinessInstaller() { LifestylePerWebRequest = false })
				.Install(new RepositoriesInstaller() { LifestylePerWebRequest = false })
				.Install(new StorageInstaller() { LifestylePerWebRequest = false })
				.Install(new PDFConverterInstaller() { LifestylePerWebRequest = false })
				.Install(new ServiceContainerInstaller());

			var service = container.Resolve<GestaoAvaliacao.Services.ExportAnalysisService>();

			var _singleTask = Task.Run(() => service.Execute());
			_singleTask.Wait();
		}
	}
}
