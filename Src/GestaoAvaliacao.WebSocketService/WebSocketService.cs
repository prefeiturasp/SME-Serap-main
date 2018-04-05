using System;
using System.ServiceProcess;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin;
using GestaoAvaliacao.WebSocket;

namespace WebSocketService
{
	public partial class WebSocketService : ServiceBase
	{
		IDisposable SignalR;
		public WebSocketService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			if (!System.Diagnostics.EventLog.SourceExists("SignalRChat"))
			{
				System.Diagnostics.EventLog.CreateEventSource(
					"SignalRChat", "Application");
			}
			eventLog1.Source = "SignalRChat";
			eventLog1.Log = "Application";

			eventLog1.WriteEntry("In OnStart");

			SignalR = WebSocketServer.Start();

		}

		protected override void OnStop()
		{
			eventLog1.WriteEntry("In OnStop");
			SignalR.Dispose();
		}
	}
}
