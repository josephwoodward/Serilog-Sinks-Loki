using System;
using System.Collections.Generic;
using Serilog.Configuration;
using Serilog.Sinks.Http;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
    public static class LokiSinkExtensions
    {
        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, LokiCredentials credentials, ILogLabelProvider labelProvider = null, LokiHttpClient httpClient = null)
            => LokiHttpImpl(sinkConfiguration, credentials, labelProvider, httpClient);

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, Func<LokiSinkConfiguration> configFactory)
            => LokiHttpImpl(sinkConfiguration, configFactory());

        private static LoggerConfiguration LokiHttpImpl(this LoggerSinkConfiguration serilogConfig, LokiSinkConfiguration lokiConfig)
        {
            var formatter = new LokiBatchFormatter();

            formatter.FormatterStrategy = lokiConfig.LokiFormatterStrategy;
            formatter.PropertiesAsLabels = lokiConfig.PropertiesAsLabels ?? new List<string>();
            formatter.PropertiesToAppend = lokiConfig.PropertiesToAppend ?? new List<string>();
            formatter.GlobalLabels = lokiConfig.LogLabelProvider != null
            ? lokiConfig.LogLabelProvider.GetLabels()
            : new List<LokiLabel>();

            LokiCredentials credentials;
            if (string.IsNullOrWhiteSpace(lokiConfig.LokiUsername))
            {
                credentials = new NoAuthCredentials(lokiConfig.LokiUrl);
            }
            else
            {
                credentials = new BasicAuthCredentials(lokiConfig.LokiUrl, lokiConfig.LokiUsername, lokiConfig.LokiPassword);
            }

            var client = lokiConfig.HttpClient ?? new LokiHttpClient();
            if (client is LokiHttpClient c)
            {
                c.SetAuthCredentials(credentials);
            }
            return serilogConfig.Http(LokiRouteBuilder.BuildPostUri(credentials.Url), batchFormatter: formatter, httpClient: client);
        }
        private static LoggerConfiguration LokiHttpImpl(this LoggerSinkConfiguration sinkConfiguration, LokiCredentials credentials, ILogLabelProvider logLabelProvider, IHttpClient httpClient)
        {
            var formatter = new LokiBatchFormatter();
            
            formatter.GlobalLabels = logLabelProvider != null ? logLabelProvider.GetLabels() : new List<LokiLabel>();

            var client = httpClient ?? new LokiHttpClient();
            if (client is LokiHttpClient c)
            {
                c.SetAuthCredentials(credentials);
            }

            return sinkConfiguration.Http(LokiRouteBuilder.BuildPostUri(credentials.Url), batchFormatter: formatter, httpClient: client);
        }

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, string serverUrl)
            => sinkConfiguration.LokiHttp(new NoAuthCredentials(serverUrl));

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, string serverUrl, string username, string password)
            => sinkConfiguration.LokiHttp(new BasicAuthCredentials(serverUrl, username, password));
    }
}
