using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Services.Logging.Log4netIntegration;
using Castle.Windsor;
using GestaoAvaliacao.FGVIntegration.Business;
using GestaoAvaliacao.FGVIntegration.Data;
using GestaoAvaliacao.FGVIntegration.Logging;

namespace GestaoAvaliacao.FGVIntegration
{
    public class FGVEnsinoMedioInstaller : IWindsorInstaller
	{

		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromAssemblyContaining<IntegracaoBusiness>()
								.BasedOn(typeof(IIntegracaoBusiness))
								.WithService.AllInterfaces());

			container.Register(Classes.FromAssemblyContaining<FGVAPIConsumer>()
								.BasedOn(typeof(IFGVAPIConsumer))
								.WithService.AllInterfaces());

			container.Register(Classes.FromAssemblyContaining<DatabaseRepository>()
								.BasedOn(typeof(IDatabaseRepository))
								.WithService.AllInterfaces());

			container.Register(Classes.FromAssemblyContaining<EolRepository>()
								.BasedOn(typeof(IEolRepository))
								.WithService.AllInterfaces());

			container.Register(Classes.FromAssemblyContaining<LoggingHelper>()
								.BasedOn(typeof(ILogging))
								.WithService.AllInterfaces());

			container.AddFacility<LoggingFacility>(act => act.LogUsing<Log4netFactory>().WithConfig("log4net.xml"));
		}
	}

}