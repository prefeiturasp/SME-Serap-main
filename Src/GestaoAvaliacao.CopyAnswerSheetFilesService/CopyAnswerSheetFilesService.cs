using GestaoAvaliacao.CopyAnswerSheetFilesService.Quartz;
using System.ServiceProcess;

namespace GestaoAvaliacao.CopyAnswerSheetFilesService
{
	public partial class CopyAnswerSheetFilesService : ServiceBase
	{
		public CopyAnswerSheetFilesService()
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
