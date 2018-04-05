using MSTech.CoreSSO.BLL;
using MSTech.CoreSSO.Entities;
using MSTech.Log;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace GestaoAvaliacao.LogFacade
{
    public static class LogFacade
	{
		public static string Path
		{
			get
			{
				if (System.Web.HttpContext.Current == null)
					return string.Concat(System.Reflection.Assembly.GetEntryAssembly().Location, "Log");
				else
					return string.Concat(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, "Log");
			}
		}

		public static string IP
		{
			get
			{
				if (System.Web.HttpContext.Current != null)
					return System.Web.HttpContext.Current.Request.UserHostAddress;
				else
					return LocalIPAddress();
			}
		}

		public static string MachineName
		{
			get
			{
				if (System.Web.HttpContext.Current != null)
					return System.Web.HttpContext.Current.Server.MachineName;
				else
					return LocalMachineName();
			}
		}

		/// <summary>
		/// Logs exceptions that are thrown on routines in this web site.
		/// In case of error, tries to log the exception on error file.
		/// </summary>
		/// <param name="ex">Exception to be saved on database</param>
		public static void SaveError(Exception ex = null, string error = null)
		{
			try
			{
				LogError logError = new LogError(Path);

				logError.SaveLogBD = delegate (string message)
				{
					LOG_Erros entity = new LOG_Erros();
					try
					{
						entity.err_descricao = message;
						if (ex != null)
						{
							entity.err_erroBase = ex.GetBaseException().Message;
							entity.err_tipoErro = ex.GetBaseException().GetType().FullName;
							entity.err_ip = IP;
							entity.err_machineName = MachineName;
							entity.err_caminhoArq = Path;
							try
							{
								entity.err_browser = String.Concat(new[] { System.Web.HttpContext.Current.Request.Browser.Browser, System.Web.HttpContext.Current.Request.Browser.MajorVersion.ToString(), System.Web.HttpContext.Current.Request.Browser.MinorVersionString });
							}
							catch
							{
								entity.err_browser = string.Empty;
							}
						}
						else
						{
							entity.err_erroBase = error;
						}
						entity.err_dataHora = DateTime.Now;

						SYS_Sistema sistema = SYS_SistemaBO.GetEntity(new SYS_Sistema() { sis_id = GestaoAvaliacao.Util.Constants.IdSistema });
						entity.sis_id = sistema.sis_id;
						entity.sis_decricao = sistema.sis_nome;

						LOG_ErrosBO.Save(entity);
					}
                    catch
                    {
                        // Sem tratamento
                    }
                };

				if (ex != null)
					logError.Log(ex, true);
				else
					logError.Log(error, true);
			}
			catch
            {
                // Sem tratamento
            }
        }

		private static string LocalIPAddress()
		{
			if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
			{
				return string.Empty;
			}

			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

			return host
				.AddressList
				.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
		}

		private static string LocalMachineName()
		{
			return Environment.MachineName;
		}
	}
}
