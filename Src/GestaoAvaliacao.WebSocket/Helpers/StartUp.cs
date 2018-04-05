using System;
using System.Collections.Generic;
using Castle.Windsor;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.WebSocket.Helpers;
using Microsoft.AspNet.SignalR;
using Owin;
using Microsoft.Owin.Cors;

namespace GestaoAvaliacao.WebSocket 
{
	public class Startup
	{
		private readonly IWindsorContainer _container;

		public Startup(IWindsorContainer _container)
		{
			this._container = _container;
		}

		public void Configuration(IAppBuilder app)
		{

			app.Map("/signalr", map =>
			{
				var resolver = new SignalRDependencyResolver(_container);

				map.UseCors(CorsOptions.AllowAll);

				var hubConfiguration = new HubConfiguration
				{
					EnableJSONP = true, // Required for IE 9 (supports only polling)
					Resolver = resolver
				};

				map.RunSignalR(hubConfiguration);
			});
		}
	}
}
