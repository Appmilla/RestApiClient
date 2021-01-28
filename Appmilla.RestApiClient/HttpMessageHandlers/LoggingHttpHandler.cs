using Appmilla.RestApiClient.Logging;
using Appmilla.RestApiClient.Logging.Interfaces;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace Appmilla.RestApiClient.HttpMessageHandlers
{
    public class LoggingHttpHandler : DelegatingHandler
    {
        private readonly ILoggingService loggingService;

        public LoggingHttpHandler(ILoggingService loggingService)
        {
            this.loggingService = loggingService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            loggingService?.LogEvent(LoggingLevel.Debug, LogEventType.ApiCall, $"Request: {request}");

            try
            {
                // base.SendAsync calls the inner handler
                var response = await base.SendAsync(request, cancellationToken);

                loggingService?.LogEvent(LoggingLevel.Debug, LogEventType.ApiCall, $"Response: {response}");

                return response;
            }
            catch (Exception ex)
            {
                loggingService?.Error($"Failed to get response: {ex}");
                throw;
            }
        }
    }
}
