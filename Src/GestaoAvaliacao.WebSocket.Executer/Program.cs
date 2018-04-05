using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.WebSocket.Executer
{
	public static class Program
	{
		static void Main()
		{
			using (WebSocketServer.Start())
			{
				Console.WriteLine("Server running");
				Console.ReadLine();
			}
		}
	}
}
