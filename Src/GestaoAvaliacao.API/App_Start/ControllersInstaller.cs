using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Web.Mvc;

namespace GestaoAvaliacao.API.App_Start
{
    public class ControllersInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromThisAssembly()
								.BasedOn<System.Web.Http.Controllers.IHttpController>()
								.LifestyleTransient());

			container.Register(Classes.FromThisAssembly()
								.BasedOn<Controller>()
								.LifestyleTransient());
		}
	}
}