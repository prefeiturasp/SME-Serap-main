using System.ServiceProcess;

namespace GestaoAvaliacao.UnzipAnswerSheetQueueService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new UnzipAnswerSheetQueueService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
