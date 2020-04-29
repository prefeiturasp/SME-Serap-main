using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using GestaoAvaliacao.FGVIntegration.FGVEnsinoMedio;

namespace GestaoAvaliacao.MappingDependence
{
	public class FGVEnsinoMedioInstaller : IWindsorInstaller
	{

		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromAssemblyContaining<IntegracaoBusiness>()
								.BasedOn(typeof(IIntegracaoBusiness))
								.WithService.AllInterfaces());

			container.Register(Classes.FromAssemblyContaining<FGVAPIClient>()
								.BasedOn(typeof(IFGVAPIClient))
								.WithService.AllInterfaces());

			container.Register(Classes.FromAssemblyContaining<DataRepository>()
								.BasedOn(typeof(IDataRepository))
								.WithService.AllInterfaces());
		}
	}

}