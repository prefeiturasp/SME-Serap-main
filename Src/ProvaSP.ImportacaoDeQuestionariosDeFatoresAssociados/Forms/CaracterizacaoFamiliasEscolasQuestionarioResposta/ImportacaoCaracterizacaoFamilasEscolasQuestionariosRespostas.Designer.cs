namespace ImportacaoDeQuestionariosSME.Forms.CaracterizacaoFamiliasEscolasQuestionarioResposta
{
    partial class ImportacaoCaracterizacaoFamilasEscolasQuestionariosRespostas
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
            this.grbCaracterizacaoFamiliasEscolasQuestionarios = new System.Windows.Forms.GroupBox();
            this.btnImportar = new System.Windows.Forms.Button();
            this.btnLocalizarArquivo = new System.Windows.Forms.Button();
            this.lblArquivo = new System.Windows.Forms.Label();
            this.txtArquivo = new System.Windows.Forms.TextBox();
            this.grbTipoQuestionario = new System.Windows.Forms.GroupBox();
            this.rdbEscola = new System.Windows.Forms.RadioButton();
            this.rdbDRE = new System.Windows.Forms.RadioButton();
            this.rdbSME = new System.Windows.Forms.RadioButton();
            this.cmbFatorAssociadoQuestionario = new System.Windows.Forms.ComboBox();
            this.lblFatorAssociadoQuestionario = new System.Windows.Forms.Label();
            this.lblEdicao = new System.Windows.Forms.Label();
            this.cmbEdicao = new System.Windows.Forms.ComboBox();
            this.btnFechar = new System.Windows.Forms.Button();
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.SuspendLayout();
            this.grbTipoQuestionario.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbCaracterizacaoFamiliasEscolasQuestionarios
            // 
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Controls.Add(this.btnImportar);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Controls.Add(this.btnLocalizarArquivo);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Controls.Add(this.lblArquivo);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Controls.Add(this.txtArquivo);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Controls.Add(this.grbTipoQuestionario);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Controls.Add(this.cmbFatorAssociadoQuestionario);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Controls.Add(this.lblFatorAssociadoQuestionario);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Controls.Add(this.lblEdicao);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Controls.Add(this.cmbEdicao);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Location = new System.Drawing.Point(12, 12);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Name = "grbCaracterizacaoFamiliasEscolasQuestionarios";
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.Size = new System.Drawing.Size(506, 204);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.TabIndex = 0;
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.TabStop = false;
            // 
            // btnImportar
            // 
            this.btnImportar.BackColor = System.Drawing.Color.LightGreen;
            this.btnImportar.Location = new System.Drawing.Point(424, 157);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(75, 23);
            this.btnImportar.TabIndex = 8;
            this.btnImportar.Text = "Importar";
            this.btnImportar.UseVisualStyleBackColor = false;
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            // 
            // btnLocalizarArquivo
            // 
            this.btnLocalizarArquivo.Location = new System.Drawing.Point(467, 128);
            this.btnLocalizarArquivo.Name = "btnLocalizarArquivo";
            this.btnLocalizarArquivo.Size = new System.Drawing.Size(32, 23);
            this.btnLocalizarArquivo.TabIndex = 7;
            this.btnLocalizarArquivo.Text = "...";
            this.btnLocalizarArquivo.UseVisualStyleBackColor = true;
            this.btnLocalizarArquivo.Click += new System.EventHandler(this.btnLocalizarArquivo_Click);
            // 
            // lblArquivo
            // 
            this.lblArquivo.AutoSize = true;
            this.lblArquivo.Location = new System.Drawing.Point(6, 112);
            this.lblArquivo.Name = "lblArquivo";
            this.lblArquivo.Size = new System.Drawing.Size(45, 14);
            this.lblArquivo.TabIndex = 6;
            this.lblArquivo.Text = "Arquivo";
            // 
            // txtArquivo
            // 
            this.txtArquivo.Location = new System.Drawing.Point(6, 129);
            this.txtArquivo.Name = "txtArquivo";
            this.txtArquivo.ReadOnly = true;
            this.txtArquivo.Size = new System.Drawing.Size(455, 20);
            this.txtArquivo.TabIndex = 5;
            // 
            // grbTipoQuestionario
            // 
            this.grbTipoQuestionario.Controls.Add(this.rdbEscola);
            this.grbTipoQuestionario.Controls.Add(this.rdbDRE);
            this.grbTipoQuestionario.Controls.Add(this.rdbSME);
            this.grbTipoQuestionario.Location = new System.Drawing.Point(6, 61);
            this.grbTipoQuestionario.Name = "grbTipoQuestionario";
            this.grbTipoQuestionario.Size = new System.Drawing.Size(494, 48);
            this.grbTipoQuestionario.TabIndex = 4;
            this.grbTipoQuestionario.TabStop = false;
            this.grbTipoQuestionario.Text = "Tipo do questionário";
            // 
            // rdbEscola
            // 
            this.rdbEscola.AutoSize = true;
            this.rdbEscola.Location = new System.Drawing.Point(423, 19);
            this.rdbEscola.Name = "rdbEscola";
            this.rdbEscola.Size = new System.Drawing.Size(57, 18);
            this.rdbEscola.TabIndex = 2;
            this.rdbEscola.TabStop = true;
            this.rdbEscola.Text = "Escola";
            this.rdbEscola.UseVisualStyleBackColor = true;
            this.rdbEscola.CheckedChanged += new System.EventHandler(this.rdbEscola_CheckedChanged);
            // 
            // rdbDRE
            // 
            this.rdbDRE.AutoSize = true;
            this.rdbDRE.Location = new System.Drawing.Point(215, 19);
            this.rdbDRE.Name = "rdbDRE";
            this.rdbDRE.Size = new System.Drawing.Size(45, 18);
            this.rdbDRE.TabIndex = 1;
            this.rdbDRE.TabStop = true;
            this.rdbDRE.Text = "DRE";
            this.rdbDRE.UseVisualStyleBackColor = true;
            this.rdbDRE.CheckedChanged += new System.EventHandler(this.rdbDRE_CheckedChanged);
            // 
            // rdbSME
            // 
            this.rdbSME.AutoSize = true;
            this.rdbSME.Checked = true;
            this.rdbSME.Location = new System.Drawing.Point(6, 19);
            this.rdbSME.Name = "rdbSME";
            this.rdbSME.Size = new System.Drawing.Size(46, 18);
            this.rdbSME.TabIndex = 0;
            this.rdbSME.TabStop = true;
            this.rdbSME.Text = "SME";
            this.rdbSME.UseVisualStyleBackColor = true;
            this.rdbSME.CheckedChanged += new System.EventHandler(this.rdbSME_CheckedChanged);
            // 
            // cmbFatorAssociadoQuestionario
            // 
            this.cmbFatorAssociadoQuestionario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFatorAssociadoQuestionario.FormattingEnabled = true;
            this.cmbFatorAssociadoQuestionario.Location = new System.Drawing.Point(143, 33);
            this.cmbFatorAssociadoQuestionario.Name = "cmbFatorAssociadoQuestionario";
            this.cmbFatorAssociadoQuestionario.Size = new System.Drawing.Size(357, 22);
            this.cmbFatorAssociadoQuestionario.TabIndex = 3;
            // 
            // lblFatorAssociadoQuestionario
            // 
            this.lblFatorAssociadoQuestionario.AutoSize = true;
            this.lblFatorAssociadoQuestionario.Location = new System.Drawing.Point(140, 16);
            this.lblFatorAssociadoQuestionario.Name = "lblFatorAssociadoQuestionario";
            this.lblFatorAssociadoQuestionario.Size = new System.Drawing.Size(86, 14);
            this.lblFatorAssociadoQuestionario.TabIndex = 2;
            this.lblFatorAssociadoQuestionario.Text = "Fator Associado";
            // 
            // lblEdicao
            // 
            this.lblEdicao.AutoSize = true;
            this.lblEdicao.Location = new System.Drawing.Point(6, 16);
            this.lblEdicao.Name = "lblEdicao";
            this.lblEdicao.Size = new System.Drawing.Size(39, 14);
            this.lblEdicao.TabIndex = 1;
            this.lblEdicao.Text = "Edição";
            // 
            // cmbEdicao
            // 
            this.cmbEdicao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEdicao.FormattingEnabled = true;
            this.cmbEdicao.Items.AddRange(new object[] {
            "2022"});
            this.cmbEdicao.Location = new System.Drawing.Point(6, 33);
            this.cmbEdicao.Name = "cmbEdicao";
            this.cmbEdicao.Size = new System.Drawing.Size(121, 22);
            this.cmbEdicao.TabIndex = 0;
            // 
            // btnFechar
            // 
            this.btnFechar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFechar.Location = new System.Drawing.Point(438, 222);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(80, 23);
            this.btnFechar.TabIndex = 1;
            this.btnFechar.Text = "Fechar";
            this.btnFechar.UseVisualStyleBackColor = true;
            this.btnFechar.Click += new System.EventHandler(this.btnFechar_Click);
            // 
            // ImportacaoCaracterizacaoFamilasEscolasQuestionariosRespostas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 257);
            this.ControlBox = false;
            this.Controls.Add(this.btnFechar);
            this.Controls.Add(this.grbCaracterizacaoFamiliasEscolasQuestionarios);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ImportacaoCaracterizacaoFamilasEscolasQuestionariosRespostas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importar Questionários Caracterização das Famílias e Escolas";
            this.Load += new System.EventHandler(this.ImportacaoCaracterizacaoFamilasEscolasQuestionariosRespostas_Load);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.ResumeLayout(false);
            this.grbCaracterizacaoFamiliasEscolasQuestionarios.PerformLayout();
            this.grbTipoQuestionario.ResumeLayout(false);
            this.grbTipoQuestionario.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbCaracterizacaoFamiliasEscolasQuestionarios;
        private System.Windows.Forms.ComboBox cmbEdicao;
        private System.Windows.Forms.Label lblEdicao;
        private System.Windows.Forms.Button btnFechar;
        private System.Windows.Forms.ComboBox cmbFatorAssociadoQuestionario;
        private System.Windows.Forms.Label lblFatorAssociadoQuestionario;
        private System.Windows.Forms.GroupBox grbTipoQuestionario;
        private System.Windows.Forms.RadioButton rdbSME;
        private System.Windows.Forms.RadioButton rdbEscola;
        private System.Windows.Forms.RadioButton rdbDRE;
        private System.Windows.Forms.TextBox txtArquivo;
        private System.Windows.Forms.Label lblArquivo;
        private System.Windows.Forms.Button btnLocalizarArquivo;
        private System.Windows.Forms.Button btnImportar;
    }
}