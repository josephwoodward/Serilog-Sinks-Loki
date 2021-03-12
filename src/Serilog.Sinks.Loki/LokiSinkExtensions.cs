using System;
using Serilog.Configuration;
using Serilog.Formatting.Display;
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

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, string serverUrl, string username, string password, ILogLabelProvider labelProvider = null, LokiHttpClient httpClient = null, string outputTemplate = LokiSinkConfiguration.DefaultTemplate, IFormatProvider formatProvider = null)
            => LokiHttpImpl(sinkConfiguration, new BasicAuthCredentials(serverUrl, username, password), labelProvider, httpClient, outputTemplate, formatProvider);

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, string serverUrl, ILogLabelProvider labelProvider = null, LokiHttpClient httpClient = null, string outputTemplate = LokiSinkConfiguration.DefaultTemplate, IFormatProvider formatProvider = null)
            => LokiHttpImpl(sinkConfiguration, new NoAuthCredentials(serverUrl), labelProvider, httpClient, outputTemplate, formatProvider);

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, LokiCredentials credentials, ILogLabelProvider labelProvider = null, LokiHttpClient httpClient = null, string outputTemplate = LokiSinkConfiguration.DefaultTemplate, IFormatProvider formatProvider = null)
            => LokiHttpImpl(sinkConfiguration, credentials, labelProvider, httpClient, outputTemplate, formatProvider);

        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, Func<LokiSinkConfiguration> configFactory)
            => LokiHttpImpl(sinkConfiguration, configFactory());

        private static LoggerConfiguration LokiHttpImpl(this LoggerSinkConfiguration serilogConfig, LokiSinkConfiguration lokiConfig)
        {
            var credentials = string.IsNullOrWhiteSpace(lokiConfig.LokiUsername)
                ? (LokiCredentials)new NoAuthCredentials(lokiConfig.LokiUrl)
                : new BasicAuthCredentials(lokiConfig.LokiUrl, lokiConfig.LokiUsername, lokiConfig.LokiPassword);

            return LokiHttpImpl(serilogConfig, credentials, lokiConfig.LogLabelProvider, lokiConfig.HttpClient, lokiConfig.OutputTemplate, lokiConfig.FormatProvider);
        }

        private static LoggerConfiguration LokiHttpImpl(
            this LoggerSinkConfiguration sinkConfiguration,
            LokiCredentials credentials,
            ILogLabelProvider logLabelProvider,
            IHttpClient httpClient,
            string outputTemplate,
            IFormatProvider formatProvider)
        {
            var formatter = new LokiBatchFormatter(logLabelProvider ?? new DefaultLogLabelProvider());
            var client = httpClient ?? new DefaultLokiHttpClient();
            if (client is LokiHttpClient c)
            {
                c.SetAuthCredentials(credentials);
            }

            return sinkConfiguration.Http(LokiRouteBuilder.BuildPostUri(credentials.Url),
                batchFormatter: formatter,
                textFormatter: new MessageTemplateTextFormatter(outputTemplate, formatProvider),
                httpClient: client);
        }
    }
}
