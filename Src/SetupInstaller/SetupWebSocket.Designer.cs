namespace SetupInstaller
{
	partial class SetupWebSocket
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
			this.btnInstalar = new System.Windows.Forms.Button();
			this.btnDesinstalar = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnInstalar
			// 
			this.btnInstalar.Location = new System.Drawing.Point(34, 68);
			this.btnInstalar.Name = "btnInstalar";
			this.btnInstalar.Size = new System.Drawing.Size(216, 54);
			this.btnInstalar.TabIndex = 2;
			this.btnInstalar.Text = "Instalar";
			this.btnInstalar.UseVisualStyleBackColor = true;
			this.btnInstalar.Click += new System.EventHandler(this.btnInstalar_Click);
			// 
			// btnDesinstalar
			// 
			this.btnDesinstalar.Location = new System.Drawing.Point(34, 140);
			this.btnDesinstalar.Name = "btnDesinstalar";
			this.btnDesinstalar.Size = new System.Drawing.Size(216, 54);
			this.btnDesinstalar.TabIndex = 3;
			this.btnDesinstalar.Text = "Desinstalar";
			this.btnDesinstalar.UseVisualStyleBackColor = true;
			this.btnDesinstalar.Click += new System.EventHandler(this.btnDesinstalar_Click);
			// 
			// SetupWebSocket
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 235);
			this.Controls.Add(this.btnDesinstalar);
			this.Controls.Add(this.btnInstalar);
			this.Name = "SetupWebSocket";
			this.Text = "Serviço Avalia+";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnInstalar;
		private System.Windows.Forms.Button btnDesinstalar;
	}
}