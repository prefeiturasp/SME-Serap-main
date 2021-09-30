using Microsoft.Extensions.Configuration;
using Sentry;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Logging
{
    public class SentryLogger : ISentryLogger
    {
        private readonly string _DSN;
        private const string WorkerCategory = "Worker";
        private const string BusinessType = "Negócio";

        public SentryLogger(IConfiguration configuration)
        {
            _DSN = configuration.GetValue<string>("Sentry:DSN");
        }

        public void Init() => SentrySdk.Init(_DSN);

        public void LogError(string message)
            => SentrySdk.AddBreadcrumb(message, WorkerCategory, BusinessType, level: Sentry.Protocol.BreadcrumbLevel.Critical);

        public void LogErrors(IEnumerable<string> messages)
        {
            foreach (var message in messages)
                LogError(message);
        }

        public void LogError(Exception ex) => SentrySdk.CaptureException(ex);
    }
}