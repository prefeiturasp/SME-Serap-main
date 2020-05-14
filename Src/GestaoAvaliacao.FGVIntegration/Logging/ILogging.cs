using Castle.Core.Logging;

namespace GestaoAvaliacao.FGVIntegration.Logging
{
    public interface ILogging
    {
        ILogger Logger { get; set; }
    }
}