using System.ServiceProcess;

namespace GestaoAvaliacao.GenerateNewCorrectionResultService
{
    public partial class GenerateNewCorrectionResultService : ServiceBase
	{
		public GenerateNewCorrectionResultService()
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
