namespace SetupInstaller
{
	partial class SetupInstall
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
			this.txtNomeServico = new System.Windows.Forms.TextBox();
			this.lblNomeProduto = new System.Windows.Forms.Label();
			this.lblCaminho = new System.Windows.Forms.Label();
			this.txtCaminho = new System.Windows.Forms.TextBox();
			this.btnProcurarCaminho = new System.Windows.Forms.Button();
			this.btnVoltar = new System.Windows.Forms.Button();
			this.btnInstall = new System.Windows.Forms.Button();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.lblProgresso = new System.Windows.Forms.Label();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.btFinalize = new System.Windows.Forms.Button();
			this.lbMessage = new System.Windows.Forms.Label();
			this.lbMessageFinalize = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtNomeServico
			// 
			this.txtNomeServico.Location = new System.Drawing.Point(82, 12);
			this.txtNomeServico.Name = "txtNomeServico";
			this.txtNomeServico.Size = new System.Drawing.Size(244, 20);
			this.txtNomeServico.TabIndex = 1;
			// 
			// lblNomeProduto
			// 
			this.lblNomeProduto.AutoSize = true;
			this.lblNomeProduto.Location = new System.Drawing.Point(12, 15);
			this.lblNomeProduto.Name = "lblNomeProduto";
			this.lblNomeProduto.Size = new System.Drawing.Size(64, 13);
			this.lblNomeProduto.TabIndex = 2;
			this.lblNomeProduto.Text = "AvaliaMais_";
			// 
			// lblCaminho
			// 
			this.lblCaminho.AutoSize = true;
			this.lblCaminho.Location = new System.Drawing.Point(12, 35);
			this.lblCaminho.Name = "lblCaminho";
			this.lblCaminho.Size = new System.Drawing.Size(185, 13);
			this.lblCaminho.TabIndex = 18;
			this.lblCaminho.Text = "Caminho raiz de instalação do serviço";
			// 
			// txtCaminho
			// 
			this.txtCaminho.Location = new System.Drawing.Point(15, 51);
			this.txtCaminho.Name = "txtCaminho";
			this.txtCaminho.Size = new System.Drawing.Size(382, 20);
			this.txtCaminho.TabIndex = 19;
			// 
			// btnProcurarCaminho
			// 
			this.btnProcurarCaminho.Location = new System.Drawing.Point(322, 77);
			this.btnProcurarCaminho.Name = "btnProcurarCaminho";
			this.btnProcurarCaminho.Size = new System.Drawing.Size(75, 23);
			this.btnProcurarCaminho.TabIndex = 21;
			this.btnProcurarCaminho.Text = "Procurar";
			this.btnProcurarCaminho.UseVisualStyleBackColor = true;
			this.btnProcurarCaminho.Click += new System.EventHandler(this.btnProcurarCaminho_Click);
			// 
			// btnVoltar
			// 
			this.btnVoltar.Location = new System.Drawing.Point(160, 189);
			this.btnVoltar.Name = "btnVoltar";
			this.btnVoltar.Size = new System.Drawing.Size(75, 23);
			this.btnVoltar.TabIndex = 22;
			this.btnVoltar.Text = "Voltar";
			this.btnVoltar.UseVisualStyleBackColor = true;
			this.btnVoltar.Click += new System.EventHandler(this.btnVoltar_Click);
			// 
			// btnInstall
			// 
			this.btnInstall.Location = new System.Drawing.Point(241, 189);
			this.btnInstall.Name = "btnInstall";
			this.btnInstall.Size = new System.Drawing.Size(75, 23);
			this.btnInstall.TabIndex = 23;
			this.btnInstall.Text = "Instalar";
			this.btnInstall.UseVisualStyleBackColor = true;
			this.btnInstall.Click += new System.EventHandler(this.btnAvancar_Click);
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(15, 160);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(382, 23);
			this.progressBar.TabIndex = 24;
			this.progressBar.Visible = false;
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
			// 
			// lblProgresso
			// 
			this.lblProgresso.AutoSize = true;
			this.lblProgresso.Location = new System.Drawing.Point(12, 144);
			this.lblProgresso.Name = "lblProgresso";
			this.lblProgresso.Size = new System.Drawing.Size(94, 13);
			this.lblProgresso.TabIndex = 25;
			this.lblProgresso.Text = "Message Progress";
			this.lblProgresso.Visible = false;
			// 
			// btFinalize
			// 
			this.btFinalize.Location = new System.Drawing.Point(322, 189);
			this.btFinalize.Name = "btFinalize";
			this.btFinalize.Size = new System.Drawing.Size(75, 23);
			this.btFinalize.TabIndex = 26;
			this.btFinalize.Text = "Finalizar";
			this.btFinalize.UseVisualStyleBackColor = true;
			this.btFinalize.Click += new System.EventHandler(this.btFinalize_Click);
			// 
			// lbMessage
			// 
			this.lbMessage.AutoSize = true;
			this.lbMessage.Location = new System.Drawing.Point(12, 118);
			this.lbMessage.Name = "lbMessage";
			this.lbMessage.Size = new System.Drawing.Size(50, 13);
			this.lbMessage.TabIndex = 27;
			this.lbMessage.Text = "Message";
			this.lbMessage.Visible = false;
			// 
			// lbMessageFinalize
			// 
			this.lbMessageFinalize.AutoSize = true;
			this.lbMessageFinalize.Location = new System.Drawing.Point(12, 131);
			this.lbMessageFinalize.Name = "lbMessageFinalize";
			this.lbMessageFinalize.Size = new System.Drawing.Size(88, 13);
			this.lbMessageFinalize.TabIndex = 28;
			this.lbMessageFinalize.Text = "Message Finalize";
			this.lbMessageFinalize.Visible = false;
			// 
			// SetupInstall
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(409, 224);
			this.Controls.Add(this.lbMessageFinalize);
			this.Controls.Add(this.lbMessage);
			this.Controls.Add(this.btFinalize);
			this.Controls.Add(this.lblProgresso);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.btnInstall);
			this.Controls.Add(this.btnVoltar);
			this.Controls.Add(this.btnProcurarCaminho);
			this.Controls.Add(this.txtCaminho);
			this.Controls.Add(this.lblCaminho);
			this.Controls.Add(this.lblNomeProduto);
			this.Controls.Add(this.txtNomeServico);
			this.Name = "SetupInstall";
			this.Text = "Instalar serviço Avalia+";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtNomeServico;
		private System.Windows.Forms.Label lblNomeProduto;
		private System.Windows.Forms.Label lblCaminho;
		private System.Windows.Forms.TextBox txtCaminho;
		private System.Windows.Forms.Button btnProcurarCaminho;
		private System.Windows.Forms.Button btnVoltar;
		private System.Windows.Forms.Button btnInstall;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.Label lblProgresso;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.Button btFinalize;
		private System.Windows.Forms.Label lbMessage;
		private System.Windows.Forms.Label lbMessageFinalize;
	}
}