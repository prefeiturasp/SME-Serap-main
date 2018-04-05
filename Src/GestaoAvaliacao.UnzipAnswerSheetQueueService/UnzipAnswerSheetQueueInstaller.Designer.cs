namespace GestaoAvaliacao.UnzipAnswerSheetQueueService
{
    partial class UnzipAnswerSheetQueueInstaller
    {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SvcInstallerServerScheduler = new System.ServiceProcess.ServiceInstaller();
			this.SvcProcessInstallerServerScheduler = new System.ServiceProcess.ServiceProcessInstaller();
			// 
			// SvcInstallerServerScheduler
			// 
			this.SvcInstallerServerScheduler.Description = "AvaliaMais UnzipAnswerSheetQueue";
			this.SvcInstallerServerScheduler.DisplayName = "AvaliaMais UnzipAnswerSheetQueue";
			this.SvcInstallerServerScheduler.ServiceName = "AvaliaMais UnzipAnswerSheetQueue";
			this.SvcInstallerServerScheduler.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			this.SvcInstallerServerScheduler.Committed += new System.Configuration.Install.InstallEventHandler(this.SvcInstallerServerScheduler_Committed);
			// 
			// SvcProcessInstallerServerScheduler
			// 
			this.SvcProcessInstallerServerScheduler.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.SvcProcessInstallerServerScheduler.Password = null;
			this.SvcProcessInstallerServerScheduler.Username = null;
            // 
            // UnzipAnswerSheetQueueInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.SvcInstallerServerScheduler,
            this.SvcProcessInstallerServerScheduler});

		}

		#endregion

		private System.ServiceProcess.ServiceInstaller SvcInstallerServerScheduler;
		private System.ServiceProcess.ServiceProcessInstaller SvcProcessInstallerServerScheduler;
	}
}