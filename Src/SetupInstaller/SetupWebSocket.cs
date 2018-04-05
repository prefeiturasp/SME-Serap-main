using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetupInstaller
{
	public partial class SetupWebSocket : Form
	{
		public SetupWebSocket()
		{
			InitializeComponent();
		}

		private void btnInstalar_Click(object sender, EventArgs e)
		{
			SetupInstall installFrm = new SetupInstall(this);
			installFrm.Show();
			this.Hide();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = @"C:\Projetos\GestaoAvaliacao\GestaoAvaliacao\Development\Dev\Src\GestaoAvaliacao.WebSocket\bin\Debug\MSTech.Config.exe";
			Process.Start(startInfo);
		}

		private void btnDesinstalar_Click(object sender, EventArgs e)
		{
			SetupUninstall uninstallFrm = new SetupUninstall(this);
			uninstallFrm.Show();
			this.Hide();
		}
	}
}
