using Castle.Windsor;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.WebSocket.Helpers;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.WebSocket
{
	public static class WebSocketServer
	{
		public static IDisposable Start()
		{
			Startup startup = new Startup(new WindsorContainer()
				.Install(new BusinessInstaller() { LifestylePerWebRequest = false })
				.Install(new RepositoriesInstaller() { LifestylePerWebRequest = false })
				.Install(new StorageInstaller() { LifestylePerWebRequest = false })
				.Install(new PDFConverterInstaller() { LifestylePerWebRequest = false })
				.Install(new HubInstaller()));

			string url = ConfigurationManager.AppSettings["WebSocketUrl"];

			return WebApp.Start(url, startup.Configuration);
		}
	}
}
