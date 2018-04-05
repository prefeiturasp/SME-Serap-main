using System.ServiceProcess;

namespace GestaoAvaliacao.AnswerSheetLotService
{
    partial class AnswerSheetLotService : ServiceBase
	{
		public AnswerSheetLotService()
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
