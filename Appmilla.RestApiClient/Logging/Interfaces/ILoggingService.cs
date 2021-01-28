using System;
using System.Collections.Generic;

namespace Appmilla.RestApiClient.Logging.Interfaces
{
    public interface ILoggingService
    {
        void Register(ILogger logger);

        void Verbose(string message = null);

        void Debug(string message = null);

        void Information(string message = null);

        void Warning(string message = null);

        void Error(Exception ex, string message = null);

        void Error(string message = null);

        void Fatal(Exception ex, string message = null);

        void LogEvent(LoggingLevel logLevel, LogEventType logEvent, string description, IDictionary<string, string> parameters = null);
    }
}
