using System.ServiceProcess;

namespace GestaoAvaliacao.CorrectionResultService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CorrectionResultService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
