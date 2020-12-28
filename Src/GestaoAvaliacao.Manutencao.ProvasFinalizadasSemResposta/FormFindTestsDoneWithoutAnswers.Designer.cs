namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta
{
    partial class FormFindTestsDoneWithoutAnswers
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
            this.dtpUpdateDateStart = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFind = new System.Windows.Forms.Button();
            this.dtgResults = new System.Windows.Forms.DataGridView();
            this.EOL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Test = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Escola = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DRE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rbtNoAnswers = new System.Windows.Forms.RadioButton();
            this.rbtMissingTheLastAnswer = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dtgResults)).BeginInit();
            this.SuspendLayout();
            // 
            // dtpUpdateDateStart
            // 
            this.dtpUpdateDateStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpUpdateDateStart.Location = new System.Drawing.Point(12, 40);
            this.dtpUpdateDateStart.Name = "dtpUpdateDateStart";
            this.dtpUpdateDateStart.Size = new System.Drawing.Size(94, 20);
            this.dtpUpdateDateStart.TabIndex = 0;
            this.dtpUpdateDateStart.Value = new System.DateTime(2020, 12, 21, 0, 0, 0, 0);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Update Date Start";
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(333, 40);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(170, 23);
            this.btnFind.TabIndex = 2;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // dtgResults
            // 
            this.dtgResults.AllowUserToAddRows = false;
            this.dtgResults.AllowUserToDeleteRows = false;
            this.dtgResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.EOL,
            this.Name,
            this.Test,
            this.Escola,
            this.DRE});
            this.dtgResults.Location = new System.Drawing.Point(16, 89);
            this.dtgResults.Name = "dtgResults";
            this.dtgResults.RowHeadersVisible = false;
            this.dtgResults.Size = new System.Drawing.Size(1143, 324);
            this.dtgResults.TabIndex = 3;
            // 
            // EOL
            // 
            this.EOL.DataPropertyName = "EolCode";
            this.EOL.HeaderText = "EOL";
            this.EOL.Name = "EOL";
            this.EOL.ReadOnly = true;
            this.EOL.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Name
            // 
            this.Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Name.DataPropertyName = "Name";
            this.Name.HeaderText = "Name";
            this.Name.Name = "Name";
            this.Name.ReadOnly = true;
            this.Name.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Test
            // 
            this.Test.DataPropertyName = "TestName";
            this.Test.HeaderText = "Test";
            this.Test.Name = "Test";
            this.Test.ReadOnly = true;
            this.Test.Width = 300;
            // 
            // Escola
            // 
            this.Escola.DataPropertyName = "SchoolName";
            this.Escola.HeaderText = "Escola";
            this.Escola.Name = "Escola";
            this.Escola.ReadOnly = true;
            this.Escola.Width = 150;
            // 
            // DRE
            // 
            this.DRE.DataPropertyName = "DreName";
            this.DRE.HeaderText = "DRE";
            this.DRE.Name = "DRE";
            this.DRE.ReadOnly = true;
            this.DRE.Width = 150;
            // 
            // rbtNoAnswers
            // 
            this.rbtNoAnswers.AutoSize = true;
            this.rbtNoAnswers.Checked = true;
            this.rbtNoAnswers.Location = new System.Drawing.Point(113, 42);
            this.rbtNoAnswers.Name = "rbtNoAnswers";
            this.rbtNoAnswers.Size = new System.Drawing.Size(82, 17);
            this.rbtNoAnswers.TabIndex = 4;
            this.rbtNoAnswers.TabStop = true;
            this.rbtNoAnswers.Text = "No Answers";
            this.rbtNoAnswers.UseVisualStyleBackColor = true;
            // 
            // rbtMissingTheLastAnswer
            // 
            this.rbtMissingTheLastAnswer.AutoSize = true;
            this.rbtMissingTheLastAnswer.Location = new System.Drawing.Point(204, 43);
            this.rbtMissingTheLastAnswer.Name = "rbtMissingTheLastAnswer";
            this.rbtMissingTheLastAnswer.Size = new System.Drawing.Size(116, 17);
            this.rbtMissingTheLastAnswer.TabIndex = 5;
            this.rbtMissingTheLastAnswer.Text = "Missing last answer";
            this.rbtMissingTheLastAnswer.UseVisualStyleBackColor = true;
            // 
            // FormFindTestsDoneWithoutAnswers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1171, 425);
            this.Controls.Add(this.rbtMissingTheLastAnswer);
            this.Controls.Add(this.rbtNoAnswers);
            this.Controls.Add(this.dtgResults);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpUpdateDateStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormFindTestsDoneWithoutAnswers";
            ((System.ComponentModel.ISupportInitialize)(this.dtgResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpUpdateDateStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.DataGridView dtgResults;
        private System.Windows.Forms.DataGridViewTextBoxColumn EOL;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Test;
        private System.Windows.Forms.DataGridViewTextBoxColumn Escola;
        private System.Windows.Forms.DataGridViewTextBoxColumn DRE;
        private System.Windows.Forms.RadioButton rbtNoAnswers;
        private System.Windows.Forms.RadioButton rbtMissingTheLastAnswer;
    }
}

