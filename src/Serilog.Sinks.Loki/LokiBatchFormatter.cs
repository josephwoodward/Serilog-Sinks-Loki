using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.Http;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
    using System.Text;

    internal class LokiBatchFormatter : IBatchFormatter 
    {
        private readonly IList<LokiLabel> _globalLabels;
        private readonly IList<string> _propertiesAsLabels;

        public LokiBatchFormatter()
        {
            _globalLabels = new List<LokiLabel>();
        }

        public LokiBatchFormatter(ILogLabelProvider logLabelProvider)
        {
            _globalLabels = logLabelProvider.GetLabels();
            _propertiesAsLabels = logLabelProvider.PropertiesAsLabels;
        }

        public void Format(IEnumerable<LogEvent> logEvents, ITextFormatter formatter, TextWriter output)
        {
            if (logEvents == null)
                throw new ArgumentNullException(nameof(logEvents));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            List<LogEvent> logs = logEvents.ToList();
            if (!logs.Any())
                return;

            var content = new LokiContent();
            foreach (LogEvent logEvent in logs)
            {
                var stream = new LokiContentStream();
                content.Streams.Add(stream);

                stream.Labels.Add(new LokiLabel("level", GetLevel(logEvent.Level)));
                foreach (LokiLabel globalLabel in _globalLabels)
                    stream.Labels.Add(new LokiLabel(globalLabel.Key, globalLabel.Value));

                var time = logEvent.Timestamp.ToString("o");

                var sb = new StringBuilder();
                sb.AppendLine(logEvent.RenderMessage());
                if (logEvent.Exception != null)
                    // AggregateException adds a Environment.Newline to the end of ToString(), so we trim it off
                    sb.AppendLine(logEvent.Exception.ToString().TrimEnd());

                foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
                {
                    // Some enrichers pass strings with quotes surrounding the values inside the string,
                    // which results in redundant quotes after serialization and a "bad request" response.
                    // To avoid this, remove all quotes from the value.
                    // We also remove any \r\n newlines and replace with \n new lines to prevent "bad request" responses
                    // We also remove backslashes and replace with forward slashes, Loki doesn't like those either
                    var propertyValue = property.Value.ToString().Replace("\"", "").Replace("\r\n", "\n").Replace("\\", "/");
                    if (_propertiesAsLabels.Contains(property.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        stream.Labels.Add(new LokiLabel(property.Key, propertyValue));
                    }
                    else
                    {
                        sb.Append($" {property.Key}={propertyValue}");
                    }
                }

                // Loki doesn't like \r\n for new line, and we can't guarantee the message doesn't have any
                // in it, so we replace \r\n with \n on the final message
                // We also flip backslashes to forward slashes, Loki doesn't like those either.
                stream.Entries.Add(new LokiEntry(time, sb.ToString().Replace("\\", "/").Replace("\r\n", "\n")));
            }

            if (content.Streams.Count > 0)
                output.Write(content.Serialize());
        }

        public void Format(IEnumerable<string> logEvents, TextWriter output)
        {
            throw new NotImplementedException();
        }

        private static string GetLevel(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Verbose: return "trace";
                case LogEventLevel.Information: return "info";
                case LogEventLevel.Fatal: return "critical";
                default: return level.ToString().ToLower();
            }
        }
    }
}