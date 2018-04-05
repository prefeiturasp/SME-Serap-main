using System;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace GestaoAvaliacao.CorrectionResultInstaller
{
    public partial class Uninstall : Form
	{
		#region Propriedades

		private readonly Form frmSetup;
		private readonly ServiceInstaller serviceInstaller;
		private bool sucesso = false;
		private string nomeServico;
		private ManagementObject managementObj = null;

		#endregion Propriedades

		public Uninstall(Form form)
		{
			InitializeComponent();
			frmSetup = form;

			serviceInstaller = new ServiceInstaller();
			serviceInstaller.BeforeUninstall += serviceInstaller_BeforeUninstall;

			CarregarServicos();

			cboServicos.ValueMember = "DisplayName";
			cboServicos.DisplayMember = "DisplayName";

			backgroundWorker.DoWork += backgroundWorker_DoWork;
			backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
		}

		#region Eventos
		private void btnVoltar_Click(object sender, EventArgs e)
		{
			frmSetup.Show();
			this.Close();
		}
		private void btUninstall_Click(object sender, EventArgs e)
		{
			progressBar.Visible = true;
			lblProgresso.Visible = true;
			lblProgresso.Text = "Desinstalando o serviço...";
			progressBar.Style = ProgressBarStyle.Marquee;
			progressBar.MarqueeAnimationSpeed = 10;
			nomeServico = cboServicos.Text;
			string mensagem;
			if (ValidarDados(out mensagem, out managementObj) && managementObj != null)
			{
				backgroundWorker.RunWorkerAsync();
			}
			else
			{
				progressBar.Style = ProgressBarStyle.Continuous;
				progressBar.MarqueeAnimationSpeed = 0;
				lblProgresso.Text = string.Empty;

				if (!string.IsNullOrEmpty(mensagem))
				{
					MessageBox.Show(mensagem);
				}
				else
				{
					mensagem = String.Format("Ocorreu um erro durante a desinstalação do {0}. ", cboServicos.Text.Trim());
					lbMessage.Text = mensagem;
					lbMessageFinalize.Text = "Clique em finalizar para fechar. ";
					lbMessage.Visible = true;
					lbMessageFinalize.Visible = true;

					progressBar.Visible = false;
					lblProgresso.Visible = false;
				}
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
				String.Format("Desinstalação do {0} realizada com sucesso. ", nomeServico) :
				String.Format("Ocorreu um erro durante a desinstalação do {0}. ", nomeServico);

			lbMessage.Text = mensagem;
			lbMessageFinalize.Text = "Clique em finalizar para fechar. ";
			lbMessage.Visible = true;
			lbMessageFinalize.Visible = true;
			progressBar.Visible = false;
			lblProgresso.Visible = false;
			lblProgresso.Visible = false;
		}
		private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			try
			{
				string objPathName = DesinstalarServico(managementObj);

				ExcluirDIretorio(objPathName);

				sucesso = true;
			}
			catch
			{
				sucesso = false;
			}
		}
		private void serviceInstaller_BeforeUninstall(object sender, InstallEventArgs e)
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
		#endregion

		#region Métodos

		/// <summary>
		/// Realiza o rollback das operações caso ocorrer algum erro.
		/// </summary>
		/// <param name="managementObj"></param>
		private void RollBack(ManagementObject managementObj)
		{
			ServiceController[] servicos = ServiceController.GetServices(Environment.MachineName);
			if (servicos != null && !servicos.Any(s => s.ServiceName.Trim() == nomeServico))
			{
				serviceInstaller.Install(new ListDictionary());
			}

			if (managementObj != null)
			{
				string objPathName = managementObj.GetPropertyValue("PathName").ToString().Replace("\"", "");

				if (!string.IsNullOrEmpty(objPathName))
				{
					DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(objPathName));

					if (!directory.Exists)
					{
						directory.Create();
						string binPath = Path.GetDirectoryName(Application.ExecutablePath);

						string[] extensions = new string[] { ".dll", ".exe", ".config" };

						string[] files = Directory.GetFiles(binPath, "*.*", SearchOption.AllDirectories).Where(p => extensions.Any(q => q.Equals(Path.GetExtension(p)))).ToArray();

						foreach (string file in files)
						{
							File.Copy(file, Path.Combine(directory.FullName, Path.GetFileName(file)), true);
						}
					}
				}
			}
		}

		/// <summary>
		/// Exclui os diretórios e arquivos referente ao serviço.
		/// </summary>
		/// <param name="caminho">Caminho do executável do serviço.</param>
		private void ExcluirDIretorio(string caminho)
		{
			DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(caminho));
			if (directory.Exists)
			{
				foreach (var file in directory.GetFiles())
				{
					while (IsFileLocked(file))
						Thread.Sleep(100);
					file.Delete();
				}
				directory.Delete();
			}

		}

		/// <summary>
		/// Desinstala o serviço.
		/// </summary>
		/// <param name="managementObj">Objeto ManagementObject referente ao serviço.</param>
		/// <returns></returns>
		private string DesinstalarServico(ManagementObject managementObj)
		{
			string caminho = managementObj.GetPropertyValue("PathName").ToString().Replace("\"", "");
			string caminhoAssembly = String.Format("/assemblypath={0}", caminho);
			String[] cmdline = { caminhoAssembly, nomeServico };
			serviceInstaller.Context = new InstallContext("", cmdline);
			serviceInstaller.ServiceName = nomeServico;
			serviceInstaller.Uninstall(null);

			return caminho;
		}

		/// <summary>
		/// Validar dados inseridos pelo usuário.
		/// </summary>
		/// <param name="mensagem">Mensagem de validação.</param>
		/// <param name="managementObj">Objeto ManagementObject referente ao serviço.</param>
		/// <returns></returns>
		private bool ValidarDados(out string mensagem, out ManagementObject managementObj)
		{
			mensagem = string.Empty;
			managementObj = new ManagementObject();

			if (string.IsNullOrEmpty(cboServicos.Text))
			{
				mensagem = "Nome do serviço é obrigatório.";
				return false;
			}

			WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_Service WHERE Name = '{0}'", cboServicos.Text.Trim()));
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(wqlObjectQuery);
			ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();

			foreach (ManagementObject obj in managementObjectCollection)
			{
				managementObj = obj;
				return true;
			}

			mensagem = "Não foi encontrado um serviço com este nome.";
			return false;
		}
		private void CarregarServicos()
		{
			System.ServiceProcess.ServiceController[] services = System.ServiceProcess.ServiceController.GetServices();

			cboServicos.Items.AddRange(services.Where(s => s.DisplayName.Contains("GestãoAvaliação_")).Select(n => new { n.DisplayName }).OrderBy(x => x.DisplayName).ToArray());
		}

		protected virtual bool IsFileLocked(FileInfo file)
		{
			FileStream stream = null;

			try
			{
				stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
			}
			catch (IOException)
			{
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				return true;
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}

			//file is not locked
			return false;
		}
		#endregion Métodos
	}
}
