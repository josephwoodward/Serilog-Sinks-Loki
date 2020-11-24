using System;
using Serilog.Sinks.Http;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
    public class LokiSinkConfiguration
    {
        internal const string DefaultTemplate = "{Message}{NewLine}{Exception}";

        public string LokiUrl { get; set; }
        public string LokiUsername { get; set; }
        public string LokiPassword { get; set; }
        public ILogLabelProvider LogLabelProvider { get; set; }
        public IHttpClient HttpClient { get; set; }
        public string OutputTemplate { get; set; } = DefaultTemplate;
        public IFormatProvider FormatProvider { get; set; }
    }
}
