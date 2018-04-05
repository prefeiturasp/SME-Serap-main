using System.ServiceProcess;

namespace GestaoAvaliacao.UnzipAnswerSheetQueueService
{
    public partial class UnzipAnswerSheetQueueService : ServiceBase
	{
		public UnzipAnswerSheetQueueService()
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
