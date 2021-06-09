using System;
using Serilog.Sinks.Http;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
    public class LokiSinkConfiguration
    {
        internal const string DefaultTemplate = "{Message:lj}{NewLine}{Exception}";

        public string LokiUrl { get; set; }
        public string LokiUsername { get; set; }
        public string LokiPassword { get; set; }
        public ILogLabelProvider LogLabelProvider { get; set; }
        public IHttpClient HttpClient { get; set; }
        public string OutputTemplate { get; set; } = DefaultTemplate;
        public IFormatProvider FormatProvider { get; set; }
        public int BatchPostingLimit { get; set; } = 1000;
        public int? QueueLimit { get; set; }
        public TimeSpan? Period { get; set; }
    }
}
