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

        public LokiBatchFormatter()
        {
            _globalLabels = new List<LokiLabel>();
        }

        public LokiBatchFormatter(IList<LokiLabel> globalLabels)
        {
            _globalLabels = globalLabels;
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

                foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
                    // Some enrichers pass strings with quotes surrounding the values inside the string,
                    // which results in redundant quotes after serialization and a "bad request" response.
                    // To avoid this, remove all quotes from the value.
                    // We also remove any \r\n newlines and replace with \n new lines to prevent "bad request" responses
                    // We also remove backslashes and replace with forward slashes, Loki doesn't like those either
                    stream.Labels.Add(new LokiLabel(property.Key, property.Value.ToString().Replace("\"", "").Replace("\r\n", "\n").Replace("\\", "/")));

                var localTime = DateTime.Now;
                var localTimeAndOffset = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));
                var time = localTimeAndOffset.ToString("o");

                var sb = new StringBuilder();
                sb.AppendLine(logEvent.RenderMessage());
                if (logEvent.Exception != null)
                {
                    var e = logEvent.Exception;
                    while (e != null)
                    {
                        sb.AppendLine(e.Message);
                        sb.AppendLine(e.StackTrace);
                        e = e.InnerException;
                    }
                }

                // Loki doesn't like \r\n for new line, and we can't guarantee the message doesn't have any
                // in it, so we replace \r\n with \n on the final message
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
            if (level == LogEventLevel.Information)
                return "info";

            return level.ToString().ToLower();
        }
    }
}