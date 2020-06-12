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
        public IList<LokiLabel> GlobalLabels { get; set; }
        public IList<string> PropertiesAsLabels { get; set; }

        public IList<string> PropertiesToAppend { get; set; }

        public LokiFormatterStrategy FormatterStrategy { get; set; } = LokiFormatterStrategy.AllPropertiesAsLabels;

        public LokiBatchFormatter() {}

        [Obsolete("Assign to LokiBathFormatter.GlobalLabels instead.")]
        public LokiBatchFormatter(IList<LokiLabel> globalLabels)
        {
            this.GlobalLabels = globalLabels?.ToList() ?? new List<LokiLabel>();
            this.PropertiesAsLabels = new List<string>();
            this.PropertiesToAppend = new List<string>();
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
                foreach (LokiLabel globalLabel in GlobalLabels)
                    stream.Labels.Add(new LokiLabel(globalLabel.Key, globalLabel.Value));

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

                foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
                {
                    // Some enrichers pass strings with quotes surrounding the values inside the string,
                    // which results in redundant quotes after serialization and a "bad request" response.
                    // To avoid this, remove all quotes from the value.
                    // We also remove any \r\n newlines and replace with \n new lines to prevent "bad request" responses
                    // We also remove backslashes and replace with forward slashes, Loki doesn't like those either
                    var propertyValue = property.Value.ToString().Replace("\"", "").Replace("\r\n", "\n").Replace("\\", "/");

                    switch (DetermineHandleActionForProperty(property.Key))
                    {
                        case HandleAction.Discard:
                            continue;
                        case HandleAction.SendAsLabel:
                            stream.Labels.Add(new LokiLabel(property.Key, propertyValue));
                            break;
                        case HandleAction.AppendToMessage:
                            sb.Append($" {property.Key}={propertyValue}");
                            break;
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
            if (level == LogEventLevel.Information)
                return "info";

            return level.ToString().ToLower();
        }

        private HandleAction DetermineHandleActionForProperty(string propertyName)
        {
            if (this.FormatterStrategy == LokiFormatterStrategy.AllPropertiesAsLabels)
            {
                return HandleAction.SendAsLabel;
            }

            if (this.FormatterStrategy == LokiFormatterStrategy.SpecificPropertiesAsLabelsAndRestDiscarded)
            {
                return this.PropertiesAsLabels.Contains(propertyName)
                ? HandleAction.SendAsLabel
                : HandleAction.Discard;
            }

            if (this.FormatterStrategy == LokiFormatterStrategy.SpecificPropertiesAsLabelsAndRestAppended)
            {
                return this.PropertiesAsLabels.Contains(propertyName)
                ? HandleAction.SendAsLabel
                : HandleAction.AppendToMessage;
            }

            // last case: this.FormatterStrategy == LokiFormatterStrategy.SpecificPropertiesAsLabelsOrAppended)
            return this.PropertiesAsLabels.Contains(propertyName)
            ? HandleAction.SendAsLabel
            : (this.PropertiesToAppend.Contains(propertyName) ? HandleAction.AppendToMessage : HandleAction.Discard);
        }

        private enum HandleAction
        {
            Discard,
            SendAsLabel,
            AppendToMessage
        }

    }
}
