using Castle.Windsor;
using GestaoAvaliacao.API.App_Start;
using GestaoAvaliacao.MappingDependence;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GestaoAvaliacao.API
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			var _container = new WindsorContainer()
								.Install(new ControllersInstaller())
								.Install(new BusinessInstaller())
								.Install(new RepositoriesInstaller())
								.Install(new StorageInstaller())
								.Install(new PDFConverterInstaller());


			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(_container));

			var controllerFactory = new WindsorControllerFactory(_container.Kernel);
			ControllerBuilder.Current.SetControllerFactory(controllerFactory);
		}
	}
}
