using System;
using System.Collections.Generic;
using Appmilla.RestApiClient.Logging.Interfaces;

namespace Appmilla.RestApiClient.Logging
{
    public class LoggingService : ILoggingService
    {
        private readonly ICollection<ILogger> loggers = new List<ILogger>();

        public void Verbose(string message = null)
        {
            foreach (var logger in loggers)
            {
                logger.Verbose(message);
            }
        }

        public void Debug(string message = null)
        {
            foreach (var logger in loggers)
            {
                logger.Debug(message);
            }
        }

        public void Information(string message = null)
        {
            foreach (var logger in loggers)
            {
                logger.Information(message);
            };
        }

        public void Warning(string message = null)
        {
            foreach (var logger in loggers)
            {
                logger.Warning(message);
            };
        }

        public void Error(string message = null)
        {
            foreach (var logger in loggers)
            {
                logger.Error(message);
            };
        }

        public void Error(Exception ex, string message = null)
        {
            foreach (var logger in loggers)
            {
                logger.Error(ex, message);
            };
        }

        public void Fatal(Exception ex, string message = null)
        {
            foreach (var logger in loggers)
            {
                logger.Fatal(ex, message);
            };
        }

        public void LogEvent(LoggingLevel logLevel, LogEventType logEvent, string description, IDictionary<string, string> parameters = null)
        {
            foreach (var logger in loggers)
            {
                logger.LogEvent(logLevel, logEvent, description, parameters);
            };
        }

        public void Register(ILogger logger)
        {
            if (!loggers.Contains(logger))
            {
                loggers.Add(logger);
            }
        }
    }
}
