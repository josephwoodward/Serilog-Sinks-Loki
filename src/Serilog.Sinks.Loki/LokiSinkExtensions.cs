using System;
using Serilog.Configuration;
using Serilog.Sinks.Http;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
    public static class LokiSinkExtensions
    {
        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, string serverUrl)
            => sinkConfiguration.LokiHttp(new NoAuthCredentials(serverUrl));

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, string serverUrl, string username, string password)
            => sinkConfiguration.LokiHttp(new BasicAuthCredentials(serverUrl, username, password));

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, LokiCredentials credentials, ILogLabelProvider labelProvider = null, LokiHttpClient httpClient = null)
            => LokiHttpImpl(sinkConfiguration, credentials, labelProvider, httpClient);

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, Func<LokiSinkConfiguration> configFactory)
            => LokiHttpImpl(sinkConfiguration, configFactory());

        private static LoggerConfiguration LokiHttpImpl(this LoggerSinkConfiguration serilogConfig, LokiSinkConfiguration lokiConfig)
        {
            var credentials = string.IsNullOrWhiteSpace(lokiConfig.LokiUsername)
                ? (LokiCredentials)new NoAuthCredentials(lokiConfig.LokiUrl)
                : new BasicAuthCredentials(lokiConfig.LokiUrl, lokiConfig.LokiUsername, lokiConfig.LokiPassword);

            return LokiHttpImpl(serilogConfig, credentials, lokiConfig.LogLabelProvider, lokiConfig.HttpClient);
        }

        private static LoggerConfiguration LokiHttpImpl(this LoggerSinkConfiguration sinkConfiguration, LokiCredentials credentials, ILogLabelProvider logLabelProvider, IHttpClient httpClient)
        {
            var formatter = new LokiBatchFormatter(logLabelProvider ?? new DefaultLogLabelProvider());
            var client = httpClient ?? new DefaultLokiHttpClient();
            if (client is LokiHttpClient c)
            {
                c.SetAuthCredentials(credentials);
            }

            return sinkConfiguration.Http(LokiRouteBuilder.BuildPostUri(credentials.Url), batchFormatter: formatter, httpClient: client);
        }
    }
}
