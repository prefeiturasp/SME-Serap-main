using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace GestaoAvaliacao.CorrectionResultInstaller
{
    public partial class Setup : Form
	{
		#region Propriedades

		private readonly ServiceInstaller serviceInstaller;
		private readonly ServiceProcessInstaller processInstaller;
		private bool sucesso = false;

		private string CaminhoInstalacao
		{
			get
			{
				return Path.Combine(txtCaminho.Text.Trim(), NomeServico);
			}
		}

		/// <summary>
		/// Retorna o nome completo do serviço.
		/// </summary>
		private string NomeServico
		{
			get
			{
				return string.Concat(lblNomeProduto.Text, txtNomeServico.Text);
			}
		}

		#endregion

		public Setup()
		{
			InitializeComponent();
			lblNomeProduto.Text = Constantes.NomeProduto;
			txtCaminho.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), Constantes.NomeEmpresa);

			serviceInstaller = new ServiceInstaller();
			processInstaller = new ServiceProcessInstaller();

			serviceInstaller.AfterInstall += serviceInstaller_AfterInstall;
			serviceInstaller.Committed += serviceInstaller_Committed;
			serviceInstaller.BeforeRollback += serviceInstaller_BeforeRollback;
		}

		#region Events

		private void btnProcurarCaminho_Click(object sender, EventArgs e)
		{
			DialogResult resultado = folderBrowserDialog.ShowDialog();
			if (resultado == DialogResult.OK)
			{
				txtCaminho.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void btnVoltar_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnInstall_Click(object sender, EventArgs e)
		{
			progressBar.Visible = true;
			lblProgresso.Visible = true;
			lblProgresso.Text = "Instalando o serviço...";
			progressBar.Style = ProgressBarStyle.Marquee;
			progressBar.MarqueeAnimationSpeed = 10;

			string mensagem;
			if (ValidarDados(out mensagem))
			{
				backgroundWorker.RunWorkerAsync(string.Format("{0}|{1}", this.NomeServico, this.CaminhoInstalacao));
			}
			else
			{
				progressBar.Style = ProgressBarStyle.Continuous;
				progressBar.MarqueeAnimationSpeed = 0;
				lblProgresso.Text = string.Empty;
				progressBar.Visible = true;
				lblProgresso.Visible = true;

				progressBar.Visible = false;
				lblProgresso.Visible = false;
				MessageBox.Show(mensagem);
			}
		}

		private void btFinalize_Click(object sender, EventArgs e)
		{
			this.Close();
		}
		private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			progressBar.Style = ProgressBarStyle.Continuous;
			progressBar.MarqueeAnimationSpeed = 0;

			string mensagem = sucesso ?
                string.Format("Instalação do {0} realizada com sucesso. ", NomeServico) :
                string.Format("Ocorreu um erro durante a instalação do {0}. ", NomeServico);

			lbMessage.Text = mensagem;
			lbMessageFinalize.Text = "Clique em finalizar para fechar. ";
			lbMessage.Visible = true;
			lbMessageFinalize.Visible = true;
			progressBar.Visible = false;
			lblProgresso.Visible = false;

		}

		private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			string[] args = e.Argument.ToString().Split('|');

			try
			{
				CopiarArquivos(args[1]);
				InstalarServico(args[0], args[1]);
				sucesso = true;
			}
			catch
			{
				sucesso = false;

				RollBack(args[0], args[1]);
				progressBar.Visible = false;
				lblProgresso.Visible = false;
			}
		}
		private void serviceInstaller_Committed(object sender, InstallEventArgs e)
		{
			ServiceInstaller si = (ServiceInstaller)sender;
			string serviceName = si.ServiceName;
			string command = " D:(A;;CCLCSWRPWPDTLOCRRC;;;SY)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWLOCRRC;;;IU)(A;;CCLCSWLOCRRC;;;SU)(A;;LCSWRPWP;;;AU)(A;;CCLCSWRPWPDTLOCRRC;;;PU)S:(AU;FA;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;WD)";

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

		private void serviceInstaller_AfterInstall(object sender, InstallEventArgs e)
		{
			ServiceInstaller si = (ServiceInstaller)sender;
			using (ServiceController sc = new ServiceController(si.ServiceName, Environment.MachineName))
			{
				if (sc.Status != ServiceControllerStatus.Running)
				{
					sc.Start();
				}
			}
		}
		private void serviceInstaller_BeforeRollback(object sender, InstallEventArgs e)
		{
			ServiceInstaller si = (ServiceInstaller)sender;
			using (ServiceController sc = new ServiceController(si.ServiceName, Environment.MachineName))
			{
				if (sc.Status == ServiceControllerStatus.Running)
				{
					sc.Stop();
					sc.WaitForStatus(ServiceControllerStatus.Stopped);
				}
			}
		}
		private void btDesinstall_Click(object sender, EventArgs e)
		{
			Form form = new Uninstall(this);
			form.Show();
		}

		#endregion

		#region Metodos

		private bool ValidarDados(out string mensagem)
		{
			mensagem = string.Empty;

			if (string.IsNullOrEmpty(NomeServico))
			{
				mensagem = "Nome do serviço é obrigatório.";
				return false;
			}

			if (string.IsNullOrEmpty(txtCaminho.Text))
			{
				mensagem = "Caminho de instalação raiz do serviço é obrigatório.";
				return false;
			}

			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service");
			ManagementObjectCollection servicos = searcher.Get();

			foreach (ManagementObject obj in servicos)
			{
                StringBuilder sb = new StringBuilder();
                if (obj["Name"].ToString().Trim().Equals(NomeServico))
                {
                    sb.Append("Já existe um serviço com este nome.\r\n");
                }

                if (obj["PathName"] != null && obj["PathName"].ToString().Trim().Equals(CaminhoInstalacao.Trim()))
                {
                    sb.Append("Já existe um serviço instalado neste caminho.");
                }

                mensagem = sb.ToString();
                if (!string.IsNullOrEmpty(mensagem))
                {
                    return false;
                }
            }

			return true;
		}

		private void CopiarArquivos(string caminhoInstalacao)
		{
			DirectoryInfo directory = new DirectoryInfo(caminhoInstalacao);
			if (!directory.Exists)
			{
				directory.Create();
			}

			string binPath = Path.GetDirectoryName(Application.ExecutablePath);

			string[] extensions = new string[] { ".dll", ".exe", ".config", ".xml", ".dat" };

			List<string> files =
				Directory.GetFiles(binPath, "*.*", SearchOption.AllDirectories)
				.Where(p => extensions.Any(e => e.Equals(Path.GetExtension(p)))).ToList();

			foreach (string file in files)
			{
				string destFile = Path.Combine(directory.FullName, Path.GetFileName(file));

				if (file.EndsWith(".config"))
				{
					// Só copia o arquivo de config se ele não existir.
					if (!File.Exists(destFile))
					{
						File.Copy(file, destFile, false);
					}
				}
				else
				{
					File.Copy(file, destFile, true);
				}
			}
		}

		private void InstalarServico(string nomeServico, string caminhoInstalacao)
		{
			processInstaller.Account = ServiceAccount.LocalSystem;
			processInstaller.Username = null;
			processInstaller.Password = null;

			string caminho = string.Format("/assemblypath={0}",
				Path.Combine(caminhoInstalacao, Constantes.NomeExecutavel));
            string[] cmdline = { caminho };

            CorrectionResultService.CorrectionResultInstaller installer = new CorrectionResultService.CorrectionResultInstaller();
			installer.Context = new InstallContext("", cmdline);
			installer.Context.Parameters.Add("nomeServico", nomeServico);
			installer.Context.Parameters.Add("versao", GetVersaoSistema());
			installer.Install(new ListDictionary());
		}

		private string GetVersaoSistema()
		{
            string strRet = string.Format("Versão: {0}", Application.ProductVersion);
			return strRet;
		}

		private void RollBack(string nomeArquivo, string caminhoInstalacao)
		{
			ServiceController[] servicos = ServiceController.GetServices(Environment.MachineName);
			if (servicos != null && servicos.Any(s => s.ServiceName.Trim() == nomeArquivo))
			{
				serviceInstaller.Uninstall(null);
			}

			if (!string.IsNullOrEmpty(caminhoInstalacao))
			{
				DirectoryInfo directory = new DirectoryInfo(caminhoInstalacao);
				if (directory.Exists)
				{
					directory.Delete(true);
				}
			}
		}

		#endregion
	}
}
