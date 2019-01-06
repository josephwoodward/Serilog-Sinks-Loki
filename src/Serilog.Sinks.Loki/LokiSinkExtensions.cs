using Serilog.Configuration;
using Serilog.Sinks.Http;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
    public static class LokiSinkExtensions
    {
        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, string requestUri)
            => LokiHttp(sinkConfiguration, requestUri, null); 

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, string requestUri, ILogLabelProvider labelProvider)
            => LokiHttp(sinkConfiguration, requestUri, labelProvider, null); 

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, string requestUri, ILogLabelProvider labelProvider, LokiHttpClient httpClient)
            => LokiHttpImpl(sinkConfiguration, requestUri, labelProvider, httpClient); 
        
        private static LoggerConfiguration LokiHttpImpl(this LoggerSinkConfiguration sinkConfiguration, string requestUri, ILogLabelProvider logLabelProvider, IHttpClient httpClient)
        {
            var formatter = logLabelProvider != null ? new LokiBatchFormatter(logLabelProvider.GetLabels()) : new LokiBatchFormatter();
            
            return sinkConfiguration.Http(requestUri, batchFormatter: formatter, httpClient: httpClient ?? new LokiHttpClient());
        }
    }
}