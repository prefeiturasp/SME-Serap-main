using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace GestaoAvaliacao.GenerateNewCorrectionResultService
{
    [RunInstaller(true)]
	public partial class GenerateNewCorrectionResultInstaller : System.Configuration.Install.Installer
	{
		public GenerateNewCorrectionResultInstaller()
		{
			InitializeComponent();
		}
		public override void Install(IDictionary stateSaver)
		{
			string nomeServico = this.Context.Parameters["nomeServico"];
			string port = this.Context.Parameters["porta"];

			if (!string.IsNullOrEmpty(nomeServico))
			{
				this.SvcInstallerServerScheduler.ServiceName = nomeServico;
				this.SvcInstallerServerScheduler.DisplayName = nomeServico;
			}

			if (!string.IsNullOrEmpty(port))
			{
				this.SvcInstallerServerScheduler.Description += ".\n Porta do scheduler: " + port;
			}

			string versao = this.Context.Parameters["versao"];
			this.SvcInstallerServerScheduler.Description += ".\n " + versao;

			base.Install(stateSaver);
		}

		protected override void OnCommitting(IDictionary savedState)
		{
			string nomeServico = this.Context.Parameters["nomeServico"];

			if (!string.IsNullOrEmpty(nomeServico))
			{
				this.SvcInstallerServerScheduler.ServiceName = nomeServico;
				this.SvcInstallerServerScheduler.DisplayName = nomeServico;
			}

			base.OnCommitting(savedState);
		}

		public override void Rollback(IDictionary savedState)
		{
			string nomeServico = this.Context.Parameters["nomeServico"];

			if (!string.IsNullOrEmpty(nomeServico))
			{
				this.SvcInstallerServerScheduler.ServiceName = nomeServico;
				this.SvcInstallerServerScheduler.DisplayName = nomeServico;
			}

			base.Rollback(savedState);
		}

		public override void Uninstall(IDictionary savedState)
		{
			string nomeServico = this.Context.Parameters["nomeServico"];

			if (!string.IsNullOrEmpty(nomeServico))
			{
				this.SvcInstallerServerScheduler.ServiceName = nomeServico;
				this.SvcInstallerServerScheduler.DisplayName = nomeServico;
			}

			base.Uninstall(savedState);
		}

		protected override void OnAfterInstall(IDictionary savedState)
		{
			try
			{
				string nomeServico = this.Context.Parameters["nomeServico"];

				System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
				appLog.Source = "GestaoAvaliacao_GenerateNewCorrectionResult";
				appLog.WriteEntry("nomeServico: " + nomeServico);

				//Inicia o serviço
				ServiceController sc = new ServiceController(SvcInstallerServerScheduler.ServiceName, Environment.MachineName);
				if (sc.Status != ServiceControllerStatus.Running)
				{
					sc.Start();
				}
			}
			finally
			{
                base.OnAfterInstall(savedState);
            }			
		}
		protected override void OnBeforeUninstall(IDictionary savedState)
		{
			try
			{
				//Para o serviço
				ServiceController sc = new ServiceController(SvcInstallerServerScheduler.ServiceName, Environment.MachineName);

				if (sc.Status == ServiceControllerStatus.Running)
				{
					sc.Stop();
					sc.WaitForStatus(ServiceControllerStatus.Stopped);
				}
			}
			finally
			{
                base.OnBeforeUninstall(savedState);
            }			
		}
		private void SvcInstallerServerScheduler_Committed(object sender, InstallEventArgs e)
		{
			string serviceName = this.SvcInstallerServerScheduler.ServiceName;
			string command = " D:(A;;CCLCSWRPWPDTLOCRRC;;;SY)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWLOCRRC;;;IU)(A;;CCLCSWLOCRRC;;;SU)(A;;LCSWRPWP;;;AU)(A;;CCLCSWRPWPDTLOCRRC;;;PU)S:(AU;FA;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;WD)";

			try
			{
				System.Diagnostics.ProcessStartInfo procStartInfo =
					new System.Diagnostics.ProcessStartInfo("sc", " sdset " + serviceName + command);

				procStartInfo.RedirectStandardOutput = true;
				procStartInfo.UseShellExecute = false;
				procStartInfo.CreateNoWindow = true;
				procStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

				System.Diagnostics.Process proc = new System.Diagnostics.Process();
				proc.StartInfo = procStartInfo;

				proc.Start();
				proc.WaitForExit();

			}
			catch
			{
                // Sem tratamento
            }
        }
	}
}
