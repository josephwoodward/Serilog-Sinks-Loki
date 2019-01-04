using System;
using System.Net.Http;
using System.Xml;
using Serilog.Configuration;
using Serilog.Sinks.Http.BatchFormatters;
using Serilog.Sinks.Http.Private.Network;
using Serilog.Sinks.Http.Private.Sinks;
using Serilog.Sinks.Http.TextFormatters;

namespace Serilog.Sinks.Loki
{
    public static class LokiSinkExtensions
    {
        public static LoggerConfiguration Loki(this LoggerSinkConfiguration sinkConfiguration, string requestUri, IFormatProvider formatProvider = null)
        {
            var httpSink = new HttpSink(requestUri, 100, TimeSpan.FromSeconds(1), new NormalTextFormatter(),
                new DefaultBatchFormatter(), new DefaultHttpClient());
            var sink = new LokiSink(httpSink, formatProvider);
            return sinkConfiguration.Sink(sink);
        }
    }
}