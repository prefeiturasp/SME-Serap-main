using System.ServiceProcess;

namespace GestaoAvaliacao.CorrectionResultService
{
    public partial class CorrectionResultService : ServiceBase
	{
		public CorrectionResultService()
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
