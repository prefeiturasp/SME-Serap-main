using GestaoAvaliacao.ExportAnalysisService.Quartz;
using System.ServiceProcess;

namespace GestaoAvaliacao.ExportAnalysisService
{
    public partial class ExportAnalysisService : ServiceBase
	{
		public ExportAnalysisService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			Scheduler.Start();
		}

		protected override void OnStop()
		{
			Scheduler.Stop();
		}
	}
}
