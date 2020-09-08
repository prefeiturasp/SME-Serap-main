namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas
{
    partial class ManutencaoDeMultiMatriculasAtivas
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
            this.dtAlunos = new System.Windows.Forms.DataGridView();
            this.AlunoId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Nome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Matricula = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnBuscarAlunos = new System.Windows.Forms.Button();
            this.dtMatriculasAtivas = new System.Windows.Forms.DataGridView();
            this.MtuId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EscolaId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlunoIdRef = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NomeDaEscola = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Turma = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TurmaId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataDaMatricula = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataDeCriacao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataDeAlteracao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAjustar = new System.Windows.Forms.Button();
            this.btnFechar = new System.Windows.Forms.Button();
            this.pgbAjustarMatriculas = new System.Windows.Forms.ProgressBar();
            this.txtAno = new System.Windows.Forms.TextBox();
            this.lblAno = new System.Windows.Forms.Label();
            this.btnAjustarSelecionado = new System.Windows.Forms.Button();
            this.lblAlunos = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dtAlunos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtMatriculasAtivas)).BeginInit();
            this.SuspendLayout();
            // 
            // dtAlunos
            // 
            this.dtAlunos.AllowUserToAddRows = false;
            this.dtAlunos.AllowUserToDeleteRows = false;
            this.dtAlunos.AllowUserToResizeColumns = false;
            this.dtAlunos.AllowUserToResizeRows = false;
            this.dtAlunos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtAlunos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AlunoId,
            this.Nome,
            this.Matricula});
            this.dtAlunos.Location = new System.Drawing.Point(12, 45);
            this.dtAlunos.Name = "dtAlunos";
            this.dtAlunos.ReadOnly = true;
            this.dtAlunos.RowHeadersVisible = false;
            this.dtAlunos.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dtAlunos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtAlunos.Size = new System.Drawing.Size(704, 150);
            this.dtAlunos.TabIndex = 0;
            this.dtAlunos.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtAlunos_CellDoubleClick);
            // 
            // AlunoId
            // 
            this.AlunoId.DataPropertyName = "AlunoId";
            this.AlunoId.HeaderText = "AlunoId";
            this.AlunoId.Name = "AlunoId";
            this.AlunoId.ReadOnly = true;
            // 
            // Nome
            // 
            this.Nome.DataPropertyName = "Nome";
            this.Nome.HeaderText = "Nome";
            this.Nome.Name = "Nome";
            this.Nome.ReadOnly = true;
            this.Nome.Width = 475;
            // 
            // Matricula
            // 
            this.Matricula.DataPropertyName = "Matricula";
            this.Matricula.HeaderText = "Matrícula";
            this.Matricula.Name = "Matricula";
            this.Matricula.ReadOnly = true;
            // 
            // btnBuscarAlunos
            // 
            this.btnBuscarAlunos.Location = new System.Drawing.Point(103, 12);
            this.btnBuscarAlunos.Name = "btnBuscarAlunos";
            this.btnBuscarAlunos.Size = new System.Drawing.Size(104, 23);
            this.btnBuscarAlunos.TabIndex = 1;
            this.btnBuscarAlunos.Text = "Buscar Alunos";
            this.btnBuscarAlunos.UseVisualStyleBackColor = true;
            this.btnBuscarAlunos.Click += new System.EventHandler(this.btnBuscarAlunos_Click);
            // 
            // dtMatriculasAtivas
            // 
            this.dtMatriculasAtivas.AllowUserToAddRows = false;
            this.dtMatriculasAtivas.AllowUserToDeleteRows = false;
            this.dtMatriculasAtivas.AllowUserToResizeColumns = false;
            this.dtMatriculasAtivas.AllowUserToResizeRows = false;
            this.dtMatriculasAtivas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtMatriculasAtivas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MtuId,
            this.EscolaId,
            this.AlunoIdRef,
            this.NomeDaEscola,
            this.Turma,
            this.TurmaId,
            this.DataDaMatricula,
            this.DataDeCriacao,
            this.DataDeAlteracao});
            this.dtMatriculasAtivas.Location = new System.Drawing.Point(12, 242);
            this.dtMatriculasAtivas.Name = "dtMatriculasAtivas";
            this.dtMatriculasAtivas.RowHeadersVisible = false;
            this.dtMatriculasAtivas.Size = new System.Drawing.Size(704, 97);
            this.dtMatriculasAtivas.TabIndex = 2;
            // 
            // MtuId
            // 
            this.MtuId.DataPropertyName = "MtuId";
            this.MtuId.HeaderText = "MtuId";
            this.MtuId.Name = "MtuId";
            this.MtuId.Width = 50;
            // 
            // EscolaId
            // 
            this.EscolaId.DataPropertyName = "EscolaId";
            this.EscolaId.HeaderText = "EscolaId";
            this.EscolaId.Name = "EscolaId";
            this.EscolaId.ReadOnly = true;
            this.EscolaId.Width = 75;
            // 
            // AlunoIdRef
            // 
            this.AlunoIdRef.DataPropertyName = "AlunoId";
            this.AlunoIdRef.HeaderText = "AlunoIdRef";
            this.AlunoIdRef.Name = "AlunoIdRef";
            this.AlunoIdRef.ReadOnly = true;
            this.AlunoIdRef.Visible = false;
            // 
            // NomeDaEscola
            // 
            this.NomeDaEscola.DataPropertyName = "NomeDaEscola";
            this.NomeDaEscola.HeaderText = "Nome da escola";
            this.NomeDaEscola.Name = "NomeDaEscola";
            this.NomeDaEscola.ReadOnly = true;
            this.NomeDaEscola.Width = 225;
            // 
            // Turma
            // 
            this.Turma.DataPropertyName = "Turma";
            this.Turma.HeaderText = "Turma";
            this.Turma.Name = "Turma";
            this.Turma.ReadOnly = true;
            // 
            // TurmaId
            // 
            this.TurmaId.DataPropertyName = "TurmaId";
            this.TurmaId.HeaderText = "TumaId";
            this.TurmaId.Name = "TurmaId";
            this.TurmaId.ReadOnly = true;
            // 
            // DataDaMatricula
            // 
            this.DataDaMatricula.DataPropertyName = "DataDaMatricula";
            this.DataDaMatricula.HeaderText = "DataDaMatricula";
            this.DataDaMatricula.Name = "DataDaMatricula";
            // 
            // DataDeCriacao
            // 
            this.DataDeCriacao.DataPropertyName = "DataDeCriacao";
            this.DataDeCriacao.HeaderText = "DataDeCriacao";
            this.DataDeCriacao.Name = "DataDeCriacao";
            // 
            // DataDeAlteracao
            // 
            this.DataDeAlteracao.DataPropertyName = "DataDeAlteracao";
            this.DataDeAlteracao.HeaderText = "DataDeAlteracao";
            this.DataDeAlteracao.Name = "DataDeAlteracao";
            // 
            // btnAjustar
            // 
            this.btnAjustar.Location = new System.Drawing.Point(12, 385);
            this.btnAjustar.Name = "btnAjustar";
            this.btnAjustar.Size = new System.Drawing.Size(104, 23);
            this.btnAjustar.TabIndex = 3;
            this.btnAjustar.Text = "Ajustar todos";
            this.btnAjustar.UseVisualStyleBackColor = true;
            this.btnAjustar.Click += new System.EventHandler(this.btnAjustar_Click);
            // 
            // btnFechar
            // 
            this.btnFechar.Location = new System.Drawing.Point(641, 385);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(75, 23);
            this.btnFechar.TabIndex = 4;
            this.btnFechar.Text = "Fechar";
            this.btnFechar.UseVisualStyleBackColor = true;
            this.btnFechar.Click += new System.EventHandler(this.btnFechar_Click);
            // 
            // pgbAjustarMatriculas
            // 
            this.pgbAjustarMatriculas.Location = new System.Drawing.Point(12, 345);
            this.pgbAjustarMatriculas.Name = "pgbAjustarMatriculas";
            this.pgbAjustarMatriculas.Size = new System.Drawing.Size(704, 23);
            this.pgbAjustarMatriculas.TabIndex = 5;
            // 
            // txtAno
            // 
            this.txtAno.Location = new System.Drawing.Point(47, 14);
            this.txtAno.Name = "txtAno";
            this.txtAno.Size = new System.Drawing.Size(50, 20);
            this.txtAno.TabIndex = 6;
            this.txtAno.Text = "2020";
            // 
            // lblAno
            // 
            this.lblAno.AutoSize = true;
            this.lblAno.Location = new System.Drawing.Point(12, 17);
            this.lblAno.Name = "lblAno";
            this.lblAno.Size = new System.Drawing.Size(29, 13);
            this.lblAno.TabIndex = 7;
            this.lblAno.Text = "Ano:";
            // 
            // btnAjustarSelecionado
            // 
            this.btnAjustarSelecionado.Location = new System.Drawing.Point(122, 385);
            this.btnAjustarSelecionado.Name = "btnAjustarSelecionado";
            this.btnAjustarSelecionado.Size = new System.Drawing.Size(138, 23);
            this.btnAjustarSelecionado.TabIndex = 8;
            this.btnAjustarSelecionado.Text = "Ajustar selecionado";
            this.btnAjustarSelecionado.UseVisualStyleBackColor = true;
            this.btnAjustarSelecionado.Click += new System.EventHandler(this.btnAjustarSelecionado_Click);
            // 
            // lblAlunos
            // 
            this.lblAlunos.AutoSize = true;
            this.lblAlunos.Location = new System.Drawing.Point(12, 198);
            this.lblAlunos.Name = "lblAlunos";
            this.lblAlunos.Size = new System.Drawing.Size(123, 13);
            this.lblAlunos.TabIndex = 9;
            this.lblAlunos.Text = "Quantidade de alunos: 0";
            // 
            // ManutencaoDeMultiMatriculasAtivas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 421);
            this.ControlBox = false;
            this.Controls.Add(this.lblAlunos);
            this.Controls.Add(this.btnAjustarSelecionado);
            this.Controls.Add(this.lblAno);
            this.Controls.Add(this.txtAno);
            this.Controls.Add(this.pgbAjustarMatriculas);
            this.Controls.Add(this.btnFechar);
            this.Controls.Add(this.btnAjustar);
            this.Controls.Add(this.dtMatriculasAtivas);
            this.Controls.Add(this.btnBuscarAlunos);
            this.Controls.Add(this.dtAlunos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ManutencaoDeMultiMatriculasAtivas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ManutencaoDeMultiMatriculasAtivas";
            ((System.ComponentModel.ISupportInitialize)(this.dtAlunos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtMatriculasAtivas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dtAlunos;
        private System.Windows.Forms.Button btnBuscarAlunos;
        private System.Windows.Forms.DataGridView dtMatriculasAtivas;
        private System.Windows.Forms.Button btnAjustar;
        private System.Windows.Forms.DataGridViewTextBoxColumn AlunoId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nome;
        private System.Windows.Forms.DataGridViewTextBoxColumn Matricula;
        private System.Windows.Forms.Button btnFechar;
        private System.Windows.Forms.ProgressBar pgbAjustarMatriculas;
        private System.Windows.Forms.TextBox txtAno;
        private System.Windows.Forms.Label lblAno;
        private System.Windows.Forms.DataGridViewTextBoxColumn MtuId;
        private System.Windows.Forms.DataGridViewTextBoxColumn EscolaId;
        private System.Windows.Forms.DataGridViewTextBoxColumn AlunoIdRef;
        private System.Windows.Forms.DataGridViewTextBoxColumn NomeDaEscola;
        private System.Windows.Forms.DataGridViewTextBoxColumn Turma;
        private System.Windows.Forms.DataGridViewTextBoxColumn TurmaId;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataDaMatricula;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataDeCriacao;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataDeAlteracao;
        private System.Windows.Forms.Button btnAjustarSelecionado;
        private System.Windows.Forms.Label lblAlunos;
    }
}