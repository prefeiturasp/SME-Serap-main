using Castle.Windsor;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.Services;
using System;
using System.Windows.Forms;

namespace GestaoAvaliacao.AnswerSheetLotExecuter
{
    public partial class CallServices : Form
	{
		private readonly IWindsorContainer container;
		public CallServices()
		{
			this.container = new WindsorContainer()
				.Install(new BusinessInstaller() { LifestylePerWebRequest = false })
				.Install(new RepositoriesInstaller() { LifestylePerWebRequest = false })
				.Install(new StorageInstaller() { LifestylePerWebRequest = false })
				.Install(new PDFConverterInstaller() { LifestylePerWebRequest = false })
				.Install(new ServiceContainerInstaller());


			InitializeComponent();
		}

		private void btnAnswerSheetLot_Click(object sender, EventArgs e)
		{
			var service = container.Resolve<AnswersheetLotService>();
			service.Execute("Local_Executer_GestaoAvaliacao.AnswerSheetLotExecuter");
		}

		private void btnExportAnalysis_Click(object sender, EventArgs e)
		{
			var service = container.Resolve<ExportAnalysisService>();
			service.Execute();
		}

        private void btnUnzipAnswerSheetQueue_Click(object sender, EventArgs e)
        {
            var service = container.Resolve<UnzipAnswerSheetQueue>();
            service.Execute();
        }

        private void btnCorrectionResult_Click(object sender, EventArgs e)
        {
            var service = container.Resolve<CorrectionResult>();
            service.Execute();
        }

        private void btnGenerateNewCorrectionResults_Click(object sender, EventArgs e)
        {
            var service = container.Resolve<GenerateNewCorrectionResult>();
            service.Execute();
        }
    }
}
