using Castle.Windsor;
using GestaoAvaliacao.App_Start;
using GestaoAvaliacao.Hubs.Tests;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.MappingDependence;
using Microsoft.AspNet.SignalR;
using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GestaoAvaliacao
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer container;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            BootstrapContainer();
            SignalRHubRegistration();

            #if DEBUG
            BundleTable.EnableOptimizations = false;
            #else 
                BundleTable.EnableOptimizations = true;
            #endif

        }

        protected void Application_End()
        {
            container.Dispose();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            ///## Importante ##
            /// Como está usando Session, precisa deste método para não ficar fazendo Looping no Redirect com o LoginSSO.
            /// Precisa deste método pois ao iniciar o pipeline o Owin ainda não entende Session. 
            
            /// When using cookie-based session state, ASP.NET does not allocate storage for session data until the Session object is used. 
            /// As a result, a new session ID is generated for each page request until the session object is accessed. 
            /// If your application requires a static session ID for the entire session, 
            /// you can either implement the Session_Start method in the application's Global.asax file and store data in the Session object to fix the session ID, 
            /// or you can use code in another part of your application to explicitly store data in the Session object.
            ///base.Session["init"] = 0;
        }

        private static void BootstrapContainer()
        {
            container = new WindsorContainer()
                .Install(new ControllersInstaller())
                .Install(new BusinessInstaller())
                .Install(new RepositoriesInstaller())
                .Install(new StorageInstaller())
                .Install(new PDFConverterInstaller())
                .Install(new UtilIntaller());


            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(container));
            
            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        private static void SignalRHubRegistration()
        {
            GlobalHost.DependencyResolver.Register(typeof(StudentTestSessionHub), () => new StudentTestSessionHub(BusinessInstaller.GetIStudentTestAccoplishmentBusinessForHub()));
        }
    }
}
