using Castle.Core.Logging;

namespace GestaoAvaliacao.FGVIntegration.Logging
{
    public class LoggingHelper : ILogging
    {

        /// <summary>
        /// O Logger correto é aplicado através do <see cref="FGVEnsinoMedioInstaller"/>
        /// </summary>
        public ILogger Logger { get; set; } = NullLogger.Instance;
    }

}