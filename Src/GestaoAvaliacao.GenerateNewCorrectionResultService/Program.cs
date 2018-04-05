using System.ServiceProcess;

namespace GestaoAvaliacao.GenerateNewCorrectionResultService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new GenerateNewCorrectionResultService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
