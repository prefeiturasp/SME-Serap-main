namespace GestaoAvaliacao.FGVIntegration
{
    partial class IntegrationForm
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
            this.tabControlGeral = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupSincronizar = new System.Windows.Forms.GroupBox();
            this.btSincEnsinoMedio = new System.Windows.Forms.Button();
            this.tabControlGeral.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupSincronizar.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlGeral
            // 
            this.tabControlGeral.Controls.Add(this.tabPage1);
            this.tabControlGeral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlGeral.Location = new System.Drawing.Point(0, 0);
            this.tabControlGeral.Name = "tabControlGeral";
            this.tabControlGeral.SelectedIndex = 0;
            this.tabControlGeral.Size = new System.Drawing.Size(452, 293);
            this.tabControlGeral.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupSincronizar);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(444, 267);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Integração FGV";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupSincronizar
            // 
            this.groupSincronizar.Controls.Add(this.btSincEnsinoMedio);
            this.groupSincronizar.Location = new System.Drawing.Point(8, 6);
            this.groupSincronizar.Name = "groupSincronizar";
            this.groupSincronizar.Size = new System.Drawing.Size(200, 178);
            this.groupSincronizar.TabIndex = 1;
            this.groupSincronizar.TabStop = false;
            this.groupSincronizar.Text = "Integração";
            // 
            // btSincEnsinoMedio
            // 
            this.btSincEnsinoMedio.Location = new System.Drawing.Point(6, 19);
            this.btSincEnsinoMedio.Name = "btSincEnsinoMedio";
            this.btSincEnsinoMedio.Size = new System.Drawing.Size(108, 23);
            this.btSincEnsinoMedio.TabIndex = 0;
            this.btSincEnsinoMedio.Text = "Ensino Médio";
            this.btSincEnsinoMedio.UseVisualStyleBackColor = true;
            this.btSincEnsinoMedio.Click += new System.EventHandler(this.btSincEnsinoMedio_Click);
            // 
            // IntegrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 293);
            this.Controls.Add(this.tabControlGeral);
            this.Name = "IntegrationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Integração de API FGV Ensino Médio";
            this.tabControlGeral.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupSincronizar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControlGeral;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupSincronizar;
        private System.Windows.Forms.Button btSincEnsinoMedio;
    }
}