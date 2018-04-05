namespace GestaoAvaliacao.UnzipAnswerSheetQueueInstaller
{
    partial class Setup
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblNomeProduto = new System.Windows.Forms.Label();
			this.txtNomeServico = new System.Windows.Forms.TextBox();
			this.lblCaminho = new System.Windows.Forms.Label();
			this.txtCaminho = new System.Windows.Forms.TextBox();
			this.btnProcurarCaminho = new System.Windows.Forms.Button();
			this.lbMessage = new System.Windows.Forms.Label();
			this.lbMessageFinalize = new System.Windows.Forms.Label();
			this.lblProgresso = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.btDesinstall = new System.Windows.Forms.Button();
			this.btFinalize = new System.Windows.Forms.Button();
			this.btnInstall = new System.Windows.Forms.Button();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.SuspendLayout();
			// 
			// lblNomeProduto
			// 
			this.lblNomeProduto.AutoSize = true;
			this.lblNomeProduto.Location = new System.Drawing.Point(9, 9);
			this.lblNomeProduto.Name = "lblNomeProduto";
			this.lblNomeProduto.Size = new System.Drawing.Size(94, 13);
			this.lblNomeProduto.TabIndex = 4;
			this.lblNomeProduto.Text = "GestaoAvaliacao_";
			// 
			// txtNomeServico
			// 
			this.txtNomeServico.Location = new System.Drawing.Point(109, 6);
			this.txtNomeServico.Name = "txtNomeServico";
			this.txtNomeServico.Size = new System.Drawing.Size(244, 20);
			this.txtNomeServico.TabIndex = 5;
			// 
			// lblCaminho
			// 
			this.lblCaminho.AutoSize = true;
			this.lblCaminho.Location = new System.Drawing.Point(9, 29);
			this.lblCaminho.Name = "lblCaminho";
			this.lblCaminho.Size = new System.Drawing.Size(185, 13);
			this.lblCaminho.TabIndex = 20;
			this.lblCaminho.Text = "Caminho raiz de instalação do serviço";
			// 
			// txtCaminho
			// 
			this.txtCaminho.Location = new System.Drawing.Point(12, 45);
			this.txtCaminho.Name = "txtCaminho";
			this.txtCaminho.Size = new System.Drawing.Size(382, 20);
			this.txtCaminho.TabIndex = 21;
			// 
			// btnProcurarCaminho
			// 
			this.btnProcurarCaminho.Location = new System.Drawing.Point(319, 71);
			this.btnProcurarCaminho.Name = "btnProcurarCaminho";
			this.btnProcurarCaminho.Size = new System.Drawing.Size(75, 23);
			this.btnProcurarCaminho.TabIndex = 23;
			this.btnProcurarCaminho.Text = "Procurar";
			this.btnProcurarCaminho.UseVisualStyleBackColor = true;
			this.btnProcurarCaminho.Click += new System.EventHandler(this.btnProcurarCaminho_Click);
			// 
			// lbMessage
			// 
			this.lbMessage.AutoSize = true;
			this.lbMessage.Location = new System.Drawing.Point(12, 93);
			this.lbMessage.Name = "lbMessage";
			this.lbMessage.Size = new System.Drawing.Size(50, 13);
			this.lbMessage.TabIndex = 29;
			this.lbMessage.Text = "Message";
			this.lbMessage.Visible = false;
			// 
			// lbMessageFinalize
			// 
			this.lbMessageFinalize.AutoSize = true;
			this.lbMessageFinalize.Location = new System.Drawing.Point(12, 106);
			this.lbMessageFinalize.Name = "lbMessageFinalize";
			this.lbMessageFinalize.Size = new System.Drawing.Size(88, 13);
			this.lbMessageFinalize.TabIndex = 30;
			this.lbMessageFinalize.Text = "Message Finalize";
			this.lbMessageFinalize.Visible = false;
			// 
			// lblProgresso
			// 
			this.lblProgresso.AutoSize = true;
			this.lblProgresso.Location = new System.Drawing.Point(12, 119);
			this.lblProgresso.Name = "lblProgresso";
			this.lblProgresso.Size = new System.Drawing.Size(94, 13);
			this.lblProgresso.TabIndex = 31;
			this.lblProgresso.Text = "Message Progress";
			this.lblProgresso.Visible = false;
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(12, 135);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(382, 23);
			this.progressBar.TabIndex = 32;
			this.progressBar.Visible = false;
			// 
			// btDesinstall
			// 
			this.btDesinstall.Location = new System.Drawing.Point(12, 164);
			this.btDesinstall.Name = "btDesinstall";
			this.btDesinstall.Size = new System.Drawing.Size(75, 23);
			this.btDesinstall.TabIndex = 36;
			this.btDesinstall.Text = "Desinstalar";
			this.btDesinstall.UseVisualStyleBackColor = true;
			this.btDesinstall.Click += new System.EventHandler(this.btDesinstall_Click);
			// 
			// btFinalize
			// 
			this.btFinalize.Location = new System.Drawing.Point(319, 164);
			this.btFinalize.Name = "btFinalize";
			this.btFinalize.Size = new System.Drawing.Size(75, 23);
			this.btFinalize.TabIndex = 37;
			this.btFinalize.Text = "Finalizar";
			this.btFinalize.UseVisualStyleBackColor = true;
			this.btFinalize.Click += new System.EventHandler(this.btFinalize_Click);
			// 
			// btnInstall
			// 
			this.btnInstall.Location = new System.Drawing.Point(238, 164);
			this.btnInstall.Name = "btnInstall";
			this.btnInstall.Size = new System.Drawing.Size(75, 23);
			this.btnInstall.TabIndex = 38;
			this.btnInstall.Text = "Instalar";
			this.btnInstall.UseVisualStyleBackColor = true;
			this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
			// 
			// Setup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(401, 195);
			this.Controls.Add(this.btnInstall);
			this.Controls.Add(this.btFinalize);
			this.Controls.Add(this.btDesinstall);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.lblProgresso);
			this.Controls.Add(this.lbMessageFinalize);
			this.Controls.Add(this.lbMessage);
			this.Controls.Add(this.btnProcurarCaminho);
			this.Controls.Add(this.txtCaminho);
			this.Controls.Add(this.lblCaminho);
			this.Controls.Add(this.txtNomeServico);
			this.Controls.Add(this.lblNomeProduto);
			this.Name = "Setup";
			this.Text = "Setup";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblNomeProduto;
		private System.Windows.Forms.TextBox txtNomeServico;
		private System.Windows.Forms.Label lblCaminho;
		private System.Windows.Forms.TextBox txtCaminho;
		private System.Windows.Forms.Button btnProcurarCaminho;
		private System.Windows.Forms.Label lbMessage;
		private System.Windows.Forms.Label lbMessageFinalize;
		private System.Windows.Forms.Label lblProgresso;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Button btDesinstall;
		private System.Windows.Forms.Button btFinalize;
		private System.Windows.Forms.Button btnInstall;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
	}
}

