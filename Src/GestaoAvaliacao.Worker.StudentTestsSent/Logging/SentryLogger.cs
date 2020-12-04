using Sentry;
using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Logging
{
    internal static class SentryLogger
    {
        private const string WorkerCategory = "Worker";
        private const string BusinessType = "Negócio";

        internal static void LogError(string message)
            => SentrySdk.AddBreadcrumb(message, WorkerCategory, BusinessType, level: Sentry.Protocol.BreadcrumbLevel.Critical);

        internal static void LogErrors(IEnumerable<string> messages)
        {
            foreach (var message in messages)
                LogError(message);
        }

        internal static void LogError(Exception ex) => SentrySdk.CaptureException(ex);
    }
}