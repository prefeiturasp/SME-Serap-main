namespace ImportacaoDeQuestionariosSME.Forms.FatoresAssociadosQuestionarioResposta
{
    partial class ImportacaoDeQuestionariosDeFatoresAssociadosQuestionarios
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbQuestionario = new System.Windows.Forms.ComboBox();
            this.lblQuestionario = new System.Windows.Forms.Label();
            this.btnImportar = new System.Windows.Forms.Button();
            this.lblAno = new System.Windows.Forms.Label();
            this.txtAno = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdbSME = new System.Windows.Forms.RadioButton();
            this.rdbEscola = new System.Windows.Forms.RadioButton();
            this.rdbDRE = new System.Windows.Forms.RadioButton();
            this.btnLocalizarTabela = new System.Windows.Forms.Button();
            this.txtArquivo = new System.Windows.Forms.TextBox();
            this.btnFechar = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnImportarFatoresAssociados = new System.Windows.Forms.Button();
            this.btnLocalizarTabelaFatoresAssociados = new System.Windows.Forms.Button();
            this.txtArquivoFatoresAssociados = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbQuestionario);
            this.groupBox1.Controls.Add(this.lblQuestionario);
            this.groupBox1.Controls.Add(this.btnImportar);
            this.groupBox1.Controls.Add(this.lblAno);
            this.groupBox1.Controls.Add(this.txtAno);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.btnLocalizarTabela);
            this.groupBox1.Controls.Add(this.txtArquivo);
            this.groupBox1.Controls.Add(this.btnFechar);
            this.groupBox1.Location = new System.Drawing.Point(12, 108);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(556, 250);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // cmbQuestionario
            // 
            this.cmbQuestionario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuestionario.FormattingEnabled = true;
            this.cmbQuestionario.Items.AddRange(new object[] {
            "21 - Resultados do 3º a 6º ano",
            "22 - Resultados do 7º a 9º ano"});
            this.cmbQuestionario.Location = new System.Drawing.Point(20, 146);
            this.cmbQuestionario.Name = "cmbQuestionario";
            this.cmbQuestionario.Size = new System.Drawing.Size(527, 22);
            this.cmbQuestionario.TabIndex = 13;
            // 
            // lblQuestionario
            // 
            this.lblQuestionario.AutoSize = true;
            this.lblQuestionario.Location = new System.Drawing.Point(17, 127);
            this.lblQuestionario.Name = "lblQuestionario";
            this.lblQuestionario.Size = new System.Drawing.Size(68, 14);
            this.lblQuestionario.TabIndex = 14;
            this.lblQuestionario.Text = "Questionário";
            // 
            // btnImportar
            // 
            this.btnImportar.BackColor = System.Drawing.Color.LightGreen;
            this.btnImportar.Location = new System.Drawing.Point(20, 212);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(135, 23);
            this.btnImportar.TabIndex = 11;
            this.btnImportar.Text = "Importar";
            this.btnImportar.UseVisualStyleBackColor = false;
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            // 
            // lblAno
            // 
            this.lblAno.AutoSize = true;
            this.lblAno.Location = new System.Drawing.Point(20, 16);
            this.lblAno.Name = "lblAno";
            this.lblAno.Size = new System.Drawing.Size(27, 14);
            this.lblAno.TabIndex = 5;
            this.lblAno.Text = "Ano";
            // 
            // txtAno
            // 
            this.txtAno.Location = new System.Drawing.Point(20, 35);
            this.txtAno.MaxLength = 4;
            this.txtAno.Name = "txtAno";
            this.txtAno.Size = new System.Drawing.Size(64, 20);
            this.txtAno.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdbSME);
            this.groupBox2.Controls.Add(this.rdbEscola);
            this.groupBox2.Controls.Add(this.rdbDRE);
            this.groupBox2.Location = new System.Drawing.Point(20, 72);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(527, 50);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tipo de questionário";
            // 
            // rdbSME
            // 
            this.rdbSME.AutoSize = true;
            this.rdbSME.Location = new System.Drawing.Point(243, 20);
            this.rdbSME.Name = "rdbSME";
            this.rdbSME.Size = new System.Drawing.Size(46, 18);
            this.rdbSME.TabIndex = 8;
            this.rdbSME.TabStop = true;
            this.rdbSME.Text = "SME";
            this.rdbSME.UseVisualStyleBackColor = true;
            // 
            // rdbEscola
            // 
            this.rdbEscola.AutoSize = true;
            this.rdbEscola.Location = new System.Drawing.Point(120, 20);
            this.rdbEscola.Name = "rdbEscola";
            this.rdbEscola.Size = new System.Drawing.Size(57, 18);
            this.rdbEscola.TabIndex = 7;
            this.rdbEscola.TabStop = true;
            this.rdbEscola.Text = "Escola";
            this.rdbEscola.UseVisualStyleBackColor = true;
            // 
            // rdbDRE
            // 
            this.rdbDRE.AutoSize = true;
            this.rdbDRE.Checked = true;
            this.rdbDRE.Location = new System.Drawing.Point(19, 20);
            this.rdbDRE.Name = "rdbDRE";
            this.rdbDRE.Size = new System.Drawing.Size(45, 18);
            this.rdbDRE.TabIndex = 6;
            this.rdbDRE.TabStop = true;
            this.rdbDRE.Text = "DRE";
            this.rdbDRE.UseVisualStyleBackColor = true;
            // 
            // btnLocalizarTabela
            // 
            this.btnLocalizarTabela.Location = new System.Drawing.Point(517, 173);
            this.btnLocalizarTabela.Name = "btnLocalizarTabela";
            this.btnLocalizarTabela.Size = new System.Drawing.Size(30, 23);
            this.btnLocalizarTabela.TabIndex = 9;
            this.btnLocalizarTabela.Text = "...";
            this.btnLocalizarTabela.UseVisualStyleBackColor = true;
            this.btnLocalizarTabela.Click += new System.EventHandler(this.btnLocalizarTabela_Click);
            // 
            // txtArquivo
            // 
            this.txtArquivo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtArquivo.Location = new System.Drawing.Point(20, 174);
            this.txtArquivo.Name = "txtArquivo";
            this.txtArquivo.ReadOnly = true;
            this.txtArquivo.Size = new System.Drawing.Size(482, 21);
            this.txtArquivo.TabIndex = 10;
            // 
            // btnFechar
            // 
            this.btnFechar.Location = new System.Drawing.Point(472, 212);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(75, 23);
            this.btnFechar.TabIndex = 12;
            this.btnFechar.Text = "Fechar";
            this.btnFechar.UseVisualStyleBackColor = true;
            this.btnFechar.Click += new System.EventHandler(this.btnFechar_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnImportarFatoresAssociados);
            this.groupBox3.Controls.Add(this.btnLocalizarTabelaFatoresAssociados);
            this.groupBox3.Controls.Add(this.txtArquivoFatoresAssociados);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(556, 90);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Fatores Associados";
            // 
            // btnImportarFatoresAssociados
            // 
            this.btnImportarFatoresAssociados.BackColor = System.Drawing.Color.LightGreen;
            this.btnImportarFatoresAssociados.Location = new System.Drawing.Point(20, 55);
            this.btnImportarFatoresAssociados.Name = "btnImportarFatoresAssociados";
            this.btnImportarFatoresAssociados.Size = new System.Drawing.Size(135, 23);
            this.btnImportarFatoresAssociados.TabIndex = 13;
            this.btnImportarFatoresAssociados.Text = "Importar";
            this.btnImportarFatoresAssociados.UseVisualStyleBackColor = false;
            this.btnImportarFatoresAssociados.Click += new System.EventHandler(this.btnImportarFatoresAssociados_Click);
            // 
            // btnLocalizarTabelaFatoresAssociados
            // 
            this.btnLocalizarTabelaFatoresAssociados.Location = new System.Drawing.Point(517, 27);
            this.btnLocalizarTabelaFatoresAssociados.Name = "btnLocalizarTabelaFatoresAssociados";
            this.btnLocalizarTabelaFatoresAssociados.Size = new System.Drawing.Size(30, 23);
            this.btnLocalizarTabelaFatoresAssociados.TabIndex = 11;
            this.btnLocalizarTabelaFatoresAssociados.Text = "...";
            this.btnLocalizarTabelaFatoresAssociados.UseVisualStyleBackColor = true;
            this.btnLocalizarTabelaFatoresAssociados.Click += new System.EventHandler(this.btnLocalizarTabelaFatoresAssociados_Click);
            // 
            // txtArquivoFatoresAssociados
            // 
            this.txtArquivoFatoresAssociados.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtArquivoFatoresAssociados.Location = new System.Drawing.Point(20, 28);
            this.txtArquivoFatoresAssociados.Name = "txtArquivoFatoresAssociados";
            this.txtArquivoFatoresAssociados.ReadOnly = true;
            this.txtArquivoFatoresAssociados.Size = new System.Drawing.Size(482, 21);
            this.txtArquivoFatoresAssociados.TabIndex = 12;
            // 
            // ImportacaoDeQuestionariosDeFatoresAssociadosQuestionarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 370);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ImportacaoDeQuestionariosDeFatoresAssociadosQuestionarios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importar questionários sobre Fatores Associados";
            this.Shown += new System.EventHandler(this.ImportacaoDeQuestionariosDeFatoresAssociados_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnFechar;
        private System.Windows.Forms.Label lblAno;
        private System.Windows.Forms.TextBox txtAno;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdbSME;
        private System.Windows.Forms.RadioButton rdbEscola;
        private System.Windows.Forms.RadioButton rdbDRE;
        private System.Windows.Forms.Button btnLocalizarTabela;
        private System.Windows.Forms.TextBox txtArquivo;
        private System.Windows.Forms.Button btnImportar;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnImportarFatoresAssociados;
        private System.Windows.Forms.Button btnLocalizarTabelaFatoresAssociados;
        private System.Windows.Forms.TextBox txtArquivoFatoresAssociados;
        private System.Windows.Forms.ComboBox cmbQuestionario;
        private System.Windows.Forms.Label lblQuestionario;
    }
}

