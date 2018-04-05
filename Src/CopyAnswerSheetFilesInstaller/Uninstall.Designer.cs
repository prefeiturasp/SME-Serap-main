namespace CopyAnswerSheetFilesInstaller
{
	partial class Uninstall
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
			this.lblNomeServico = new System.Windows.Forms.Label();
			this.cboServicos = new System.Windows.Forms.ComboBox();
			this.lbMessage = new System.Windows.Forms.Label();
			this.lbMessageFinalize = new System.Windows.Forms.Label();
			this.lblProgresso = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.btFinalize = new System.Windows.Forms.Button();
			this.btUninstall = new System.Windows.Forms.Button();
			this.btnVoltar = new System.Windows.Forms.Button();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// lblNomeServico
			// 
			this.lblNomeServico.AutoSize = true;
			this.lblNomeServico.Location = new System.Drawing.Point(12, 9);
			this.lblNomeServico.Name = "lblNomeServico";
			this.lblNomeServico.Size = new System.Drawing.Size(87, 13);
			this.lblNomeServico.TabIndex = 13;
			this.lblNomeServico.Text = "Nome do serviço";
			// 
			// cboServicos
			// 
			this.cboServicos.FormattingEnabled = true;
			this.cboServicos.Location = new System.Drawing.Point(12, 25);
			this.cboServicos.Name = "cboServicos";
			this.cboServicos.Size = new System.Drawing.Size(393, 21);
			this.cboServicos.TabIndex = 19;
			// 
			// lbMessage
			// 
			this.lbMessage.AutoSize = true;
			this.lbMessage.Location = new System.Drawing.Point(12, 63);
			this.lbMessage.Name = "lbMessage";
			this.lbMessage.Size = new System.Drawing.Size(50, 13);
			this.lbMessage.TabIndex = 31;
			this.lbMessage.Text = "Message";
			this.lbMessage.Visible = false;
			// 
			// lbMessageFinalize
			// 
			this.lbMessageFinalize.AutoSize = true;
			this.lbMessageFinalize.Location = new System.Drawing.Point(12, 76);
			this.lbMessageFinalize.Name = "lbMessageFinalize";
			this.lbMessageFinalize.Size = new System.Drawing.Size(88, 13);
			this.lbMessageFinalize.TabIndex = 32;
			this.lbMessageFinalize.Text = "Message Finalize";
			this.lbMessageFinalize.Visible = false;
			// 
			// lblProgresso
			// 
			this.lblProgresso.AutoSize = true;
			this.lblProgresso.Location = new System.Drawing.Point(12, 89);
			this.lblProgresso.Name = "lblProgresso";
			this.lblProgresso.Size = new System.Drawing.Size(94, 13);
			this.lblProgresso.TabIndex = 33;
			this.lblProgresso.Text = "Message Progress";
			this.lblProgresso.Visible = false;
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(15, 105);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(390, 25);
			this.progressBar.TabIndex = 34;
			this.progressBar.Visible = false;
			// 
			// btFinalize
			// 
			this.btFinalize.Location = new System.Drawing.Point(330, 136);
			this.btFinalize.Name = "btFinalize";
			this.btFinalize.Size = new System.Drawing.Size(75, 23);
			this.btFinalize.TabIndex = 35;
			this.btFinalize.Text = "Finalizar";
			this.btFinalize.UseVisualStyleBackColor = true;
			this.btFinalize.Click += new System.EventHandler(this.btFinalize_Click);
			// 
			// btUninstall
			// 
			this.btUninstall.Location = new System.Drawing.Point(249, 136);
			this.btUninstall.Name = "btUninstall";
			this.btUninstall.Size = new System.Drawing.Size(75, 23);
			this.btUninstall.TabIndex = 36;
			this.btUninstall.Text = "Desinstalar";
			this.btUninstall.UseVisualStyleBackColor = true;
			this.btUninstall.Click += new System.EventHandler(this.btUninstall_Click);
			// 
			// btnVoltar
			// 
			this.btnVoltar.Location = new System.Drawing.Point(168, 136);
			this.btnVoltar.Name = "btnVoltar";
			this.btnVoltar.Size = new System.Drawing.Size(75, 23);
			this.btnVoltar.TabIndex = 37;
			this.btnVoltar.Text = "Voltar";
			this.btnVoltar.UseVisualStyleBackColor = true;
			this.btnVoltar.Click += new System.EventHandler(this.btnVoltar_Click);
			// 
			// Uninstall
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(411, 167);
			this.Controls.Add(this.btnVoltar);
			this.Controls.Add(this.btUninstall);
			this.Controls.Add(this.btFinalize);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.lblProgresso);
			this.Controls.Add(this.lbMessageFinalize);
			this.Controls.Add(this.lbMessage);
			this.Controls.Add(this.cboServicos);
			this.Controls.Add(this.lblNomeServico);
			this.Name = "Uninstall";
			this.Text = "Uninstall";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblNomeServico;
		private System.Windows.Forms.ComboBox cboServicos;
		private System.Windows.Forms.Label lbMessage;
		private System.Windows.Forms.Label lbMessageFinalize;
		private System.Windows.Forms.Label lblProgresso;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Button btFinalize;
		private System.Windows.Forms.Button btUninstall;
		private System.Windows.Forms.Button btnVoltar;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
	}
}