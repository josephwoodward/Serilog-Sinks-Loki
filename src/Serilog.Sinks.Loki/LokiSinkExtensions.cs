using Serilog.Configuration;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
    public static class LokiSinkExtensions
    {
        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, string requestUri)
            => LokiHttp(sinkConfiguration, requestUri, null); 


        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, string requestUri, ILogLabelProvider logLabelProvider)
        {
            var formatter = logLabelProvider != null ? new LokiBatchFormatter(logLabelProvider.GetLabels()) : new LokiBatchFormatter();
            return sinkConfiguration.Http(requestUri, batchFormatter: formatter, httpClient: new LokiHttpClient());
        }
    }
}