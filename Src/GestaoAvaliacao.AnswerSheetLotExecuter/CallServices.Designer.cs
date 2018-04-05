namespace GestaoAvaliacao.AnswerSheetLotExecuter
{
    partial class CallServices
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
            this.btnAnswerSheetLot = new System.Windows.Forms.Button();
            this.btnExportAnalysis = new System.Windows.Forms.Button();
            this.btnUnzipAnswerSheetQueue = new System.Windows.Forms.Button();
            this.btnCorrectionResult = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAnswerSheetLot
            // 
            this.btnAnswerSheetLot.Location = new System.Drawing.Point(74, 23);
            this.btnAnswerSheetLot.Name = "btnAnswerSheetLot";
            this.btnAnswerSheetLot.Size = new System.Drawing.Size(170, 23);
            this.btnAnswerSheetLot.TabIndex = 0;
            this.btnAnswerSheetLot.Text = "AnswerSheetLot";
            this.btnAnswerSheetLot.UseVisualStyleBackColor = true;
            this.btnAnswerSheetLot.Click += new System.EventHandler(this.btnAnswerSheetLot_Click);
            // 
            // btnExportAnalysis
            // 
            this.btnExportAnalysis.Location = new System.Drawing.Point(74, 52);
            this.btnExportAnalysis.Name = "btnExportAnalysis";
            this.btnExportAnalysis.Size = new System.Drawing.Size(170, 23);
            this.btnExportAnalysis.TabIndex = 1;
            this.btnExportAnalysis.Text = "ExportAnalysis";
            this.btnExportAnalysis.UseVisualStyleBackColor = true;
            this.btnExportAnalysis.Click += new System.EventHandler(this.btnExportAnalysis_Click);
            // 
            // btnUnzipAnswerSheetQueue
            // 
            this.btnUnzipAnswerSheetQueue.Location = new System.Drawing.Point(74, 81);
            this.btnUnzipAnswerSheetQueue.Name = "btnUnzipAnswerSheetQueue";
            this.btnUnzipAnswerSheetQueue.Size = new System.Drawing.Size(170, 23);
            this.btnUnzipAnswerSheetQueue.TabIndex = 5;
            this.btnUnzipAnswerSheetQueue.Text = "UnzipAnswerSheetQueue";
            this.btnUnzipAnswerSheetQueue.UseVisualStyleBackColor = true;
            this.btnUnzipAnswerSheetQueue.Click += new System.EventHandler(this.btnUnzipAnswerSheetQueue_Click);
            // 
            // btnCorrectionResult
            // 
            this.btnCorrectionResult.Location = new System.Drawing.Point(74, 110);
            this.btnCorrectionResult.Name = "btnCorrectionResult";
            this.btnCorrectionResult.Size = new System.Drawing.Size(170, 23);
            this.btnCorrectionResult.TabIndex = 6;
            this.btnCorrectionResult.Text = "CorrectionResult";
            this.btnCorrectionResult.UseVisualStyleBackColor = true;
            this.btnCorrectionResult.Click += new System.EventHandler(this.btnCorrectionResult_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(74, 139);
            this.button1.Name = "btnGenerateNewCorrectionResults";
            this.button1.Size = new System.Drawing.Size(170, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "GenerateNewCorrectionResults";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnGenerateNewCorrectionResults_Click);
            // 
            // CallServices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnCorrectionResult);
            this.Controls.Add(this.btnUnzipAnswerSheetQueue);
            this.Controls.Add(this.btnExportAnalysis);
            this.Controls.Add(this.btnAnswerSheetLot);
            this.Name = "CallServices";
            this.Text = "Gestao Avaliacao Services";
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnAnswerSheetLot;
		private System.Windows.Forms.Button btnExportAnalysis;
        private System.Windows.Forms.Button btnUnzipAnswerSheetQueue;
        private System.Windows.Forms.Button btnCorrectionResult;
        private System.Windows.Forms.Button button1;
    }
}