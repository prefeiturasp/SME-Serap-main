using Castle.Core.Logging;

namespace GestaoAvaliacao.FGVIntegration.Logging
{
    public abstract class LoggingBase : ILogging
    {

        /// <summary>
        /// O Logger correto é aplicado através do <see cref="FGVEnsinoMedioInstaller"/>
        /// </summary>
        public ILogger Logger { get; set; } = NullLogger.Instance;
    }

}