using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Parsing;
using Serilog.Sinks.Http;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
    using System.Text;

    internal class LokiBatchFormatter : IBatchFormatter
    {
        public ILogLabelProvider LogLabelProvider { get; }

        public LokiBatchFormatter()
        {
            this.LogLabelProvider = new DefaultLogLabelProvider();
        }

        public LokiBatchFormatter(ILogLabelProvider logLabelProvider)
        {
            this.LogLabelProvider = logLabelProvider;
        }

        [Obsolete("Assign to LokiBatchFormatter.GlobalLabels instead.")]
        public LokiBatchFormatter(IList<LokiLabel> globalLabels)
        {
            this.LogLabelProvider = new DefaultLogLabelProvider(globalLabels);
        }

        // This avoids additional quoting as described in https://github.com/serilog/serilog/issues/936
        private static void RenderMessage(TextWriter tw, LogEvent logEvent)
        {
            bool IsString(LogEventPropertyValue pv)
            {
                return pv is ScalarValue sv && sv.Value is string;
            }

            foreach(var t in logEvent.MessageTemplate.Tokens)
            {
                if (t is PropertyToken pt &&
                    logEvent.Properties.TryGetValue(pt.PropertyName, out var propVal) &&
                    IsString(propVal))
                    tw.Write(((ScalarValue)propVal).Value);
                else
                    t.Render(logEvent.Properties, tw);
            }
            tw.Write('\n');
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

            var streamsDictionary = new Dictionary<string, LokiContentStream>();
            foreach (LogEvent logEvent in logs)
            {
                var labels = new List<LokiLabel>();

                foreach (LokiLabel globalLabel in this.LogLabelProvider.GetLabels())
                    labels.Add(new LokiLabel(globalLabel.Key, globalLabel.Value));

                var time = logEvent.Timestamp.ToString("o");
                var sb = new StringBuilder();
                using (var tw = new StringWriter(sb))
                {
                    RenderMessage(tw, logEvent);
                }
                if (logEvent.Exception != null)
                    // AggregateException adds a Environment.Newline to the end of ToString(), so we trim it off
                    sb.AppendLine(logEvent.Exception.ToString().TrimEnd());

                HandleProperty("level", GetLevel(logEvent.Level), labels, sb);
                foreach (KeyValuePair<string, LogEventPropertyValue> property in logEvent.Properties)
                {
                    HandleProperty(property.Key, property.Value.ToString(), labels, sb);
                }

                // Order the labels so they always get the same chunk in loki
                labels = labels.OrderBy(l => l.Key).ToList();
                var key = string.Join(",", labels.Select(l => $"{l.Key}={l.Value}"));
                if (!streamsDictionary.TryGetValue(key, out var stream))
                {
                    streamsDictionary.Add(key, stream = new LokiContentStream());
                    stream.Labels.AddRange(labels);
                }

                // Loki doesn't like \r\n for new line, and we can't guarantee the message doesn't have any
                // in it, so we replace \r\n with \n on the final message
                stream.Entries.Add(new LokiEntry(time, sb.ToString().Replace("\r\n", "\n")));
            }

            if (streamsDictionary.Count > 0)
            {
                var content = new LokiContent
                {
                    Streams = streamsDictionary.Values.ToList()
                };
                output.Write(content.Serialize());
            }
        }

        private void HandleProperty(string name, string value, ICollection<LokiLabel> labels, StringBuilder sb)
        {
            // Some enrichers pass strings with quotes surrounding the values inside the string,
            // which results in redundant quotes after serialization and a "bad request" response.
            // To avoid this, remove all quotes from the value.
            // We also remove any \r\n newlines and replace with \n new lines to prevent "bad request" responses
            // We also remove backslashes and replace with forward slashes, Loki doesn't like those either
            value = value.Replace("\r\n", "\n");

            switch (DetermineHandleActionForProperty(name))
            {
                case HandleAction.Discard: return;
                case HandleAction.SendAsLabel:
                    value = value.Replace("\"", "").Replace("\\", "/");
                    labels.Add(new LokiLabel(name, value));
                    break;
                case HandleAction.AppendToMessage:
                    sb.Append($" {name}={value}");
                    break;
            }
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

        private HandleAction DetermineHandleActionForProperty(string propertyName)
        {
            var provider = this.LogLabelProvider;
            switch (provider.FormatterStrategy)
            {
                case LokiFormatterStrategy.AllPropertiesAsLabels:
                    return HandleAction.SendAsLabel;

                case LokiFormatterStrategy.SpecificPropertiesAsLabelsAndRestDiscarded:
                    return provider.PropertiesAsLabels.Contains(propertyName)
                        ? HandleAction.SendAsLabel
                        : HandleAction.Discard;

                case LokiFormatterStrategy.SpecificPropertiesAsLabelsAndRestAppended:
                    return provider.PropertiesAsLabels.Contains(propertyName)
                        ? HandleAction.SendAsLabel
                        : HandleAction.AppendToMessage;

                //case LokiFormatterStrategy.SpecificPropertiesAsLabelsOrAppended:
                default:
                    return provider.PropertiesAsLabels.Contains(propertyName)
                        ? HandleAction.SendAsLabel
                        : provider.PropertiesToAppend.Contains(propertyName)
                            ? HandleAction.AppendToMessage
                            : HandleAction.Discard;
            }
        }

        private enum HandleAction
        {
            Discard,
            SendAsLabel,
            AppendToMessage
        }
    }
}
