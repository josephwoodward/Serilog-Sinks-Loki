using System.Collections.Generic;
using Serilog.Sinks.Http;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
    public class LokiSinkConfiguration
    {
        public string LokiUrl { get; set; }
        public string LokiUsername { get; set; }
        public string LokiPassword { get; set; }
        public ILogLabelProvider LogLabelProvider { get; set; }
        public IHttpClient HttpClient { get; set; }
    }
}
