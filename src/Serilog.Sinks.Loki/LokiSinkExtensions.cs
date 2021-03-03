using System;
using Serilog.Configuration;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Sinks.Http;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
    public static class LokiSinkExtensions
    {
        public static LoggerConfiguration LokiHttp(this LoggerSinkConfiguration sinkConfiguration, Func<LokiSinkConfiguration> configFactory)
            => LokiHttpImpl(sinkConfiguration, configFactory());

        private static LoggerConfiguration LokiHttpImpl(this LoggerSinkConfiguration serilogConfig, LokiSinkConfiguration lokiConfig)
        {
            var credentials = string.IsNullOrWhiteSpace(lokiConfig.LokiUsername)
                ? (LokiCredentials)new NoAuthCredentials(lokiConfig.LokiUrl)
                : new BasicAuthCredentials(lokiConfig.LokiUrl, lokiConfig.LokiUsername, lokiConfig.LokiPassword);

            return LokiHttpImpl(serilogConfig, credentials, lokiConfig.LogLabelProvider, lokiConfig.HttpClient,
                lokiConfig.OutputTemplate, lokiConfig.FormatProvider, lokiConfig.TextFormatter, lokiConfig.BatchPostingLimit,
                lokiConfig.QueueLimit, lokiConfig.Period);
        }

        private static LoggerConfiguration LokiHttpImpl(
            this LoggerSinkConfiguration sinkConfiguration,
            LokiCredentials credentials,
            ILogLabelProvider logLabelProvider,
            IHttpClient httpClient,
            string outputTemplate,
            IFormatProvider formatProvider,
            ITextFormatter textFormatter,
            int batchPostingLimit,
            int? queueLimit,
            TimeSpan? period)
        {
            var formatter = new LokiBatchFormatter(logLabelProvider ?? new DefaultLogLabelProvider());
            var client = httpClient ?? new DefaultLokiHttpClient();
            if (client is LokiHttpClient c)
            {
                c.SetAuthCredentials(credentials);
            }

            return sinkConfiguration.Http(LokiRouteBuilder.BuildPostUri(credentials.Url),
                batchFormatter: formatter,
                httpClient: client,
                textFormatter: textFormatter ?? new MessageTemplateTextFormatter(outputTemplate, formatProvider),
                batchPostingLimit: batchPostingLimit,
                queueLimit: queueLimit,
                period: period);
        }
    }
}
