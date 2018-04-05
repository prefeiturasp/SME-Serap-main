using System.ServiceProcess;

namespace GestaoAvaliacao.AnswerSheetLotService
{
    static class Program
	{
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new AnswerSheetLotService()
			};
			ServiceBase.Run(ServicesToRun);
		}
	}
}
