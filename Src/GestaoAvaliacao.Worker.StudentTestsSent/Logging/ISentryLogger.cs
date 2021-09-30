using System;
using System.Collections.Generic;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Logging
{
    public interface ISentryLogger
    {
        void Init();
        void LogError(string message);
        void LogErrors(IEnumerable<string> messages);
        void LogError(Exception ex);
    }
}