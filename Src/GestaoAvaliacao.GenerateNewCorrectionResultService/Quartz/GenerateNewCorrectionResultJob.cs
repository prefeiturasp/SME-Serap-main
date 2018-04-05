using Castle.Windsor;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.Services;
using Quartz;

namespace GestaoAvaliacao.GenerateNewCorrectionResultService
{
    [DisallowConcurrentExecution]
    public class GenerateNewCorrectionResultJob : IJob
	{
        public void Execute(IJobExecutionContext context)
        {
            IWindsorContainer container = new WindsorContainer()
                .Install(new BusinessInstaller() { LifestylePerWebRequest = false })
                .Install(new RepositoriesInstaller() { LifestylePerWebRequest = false })
                .Install(new StorageInstaller() { LifestylePerWebRequest = false })
                .Install(new PDFConverterInstaller() { LifestylePerWebRequest = false })
                .Install(new ServiceContainerInstaller());

            var service = container.Resolve<GestaoAvaliacao.Services.GenerateNewCorrectionResult>();
            service.Execute();
        }
	}
}
