using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace GestaoAvaliacao.Services
{
    public class ServiceContainerInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromThisAssembly()
								.InNamespace("GestaoAvaliacao.Services")
								.WithServiceDefaultInterfaces());
		}
	}
}
