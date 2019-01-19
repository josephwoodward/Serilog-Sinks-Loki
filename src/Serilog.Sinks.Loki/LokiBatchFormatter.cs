using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.Http;
using Serilog.Sinks.Http.BatchFormatters;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
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
            
            var logs = logEvents.ToList();
            if (!logs.Any())
                return;

            var count = 0;
            
            output.Write("{\"streams\":[");
            foreach (var logEvent in logs)
            {
                count++;
                output.Write("{");
                
                var localTime = DateTime.Now;
                var localTimeAndOffset = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));
                
                var time = localTimeAndOffset.ToString("o");
                
                if (logEvent.Level == LogEventLevel.Error)
                    Console.WriteLine("Ooops");
                
                // Labels
                output.Write("\"labels\":\"{");
                AddLabel(output, "level", logEvent.Level.ToString().ToLower());
                
                foreach (var label in _globalLabels)
                {
                    output.Write(",");
                    AddLabel(output, label.Key, label.Value);
                        
                }
                
                output.Write("}\",");
                output.Write("\"entries\":[");

                if (logEvent.Properties.Any())
                {
                    foreach (var eventProperty in logEvent.Properties)
                    {
/*                        output.Write(",");
                        AddEventPropertyAsLabel(output, eventProperty.Key, eventProperty.Value);*/
                    } 
                }

                var e = new LokiEntry
                {
                    Ts = time,
                    Line = logEvent.RenderMessage()
                };

                if (logEvent.Exception != null)
                {
                    #if NETSTANDARD2_0 
                    e.Line += "\n\n" + logEvent.Exception.ToStringDemystified();
                    #else
                    e.Line += "\n\n" + logEvent.Exception.ToString();
                    #endif
                }
                
                var entry = JsonConvert.SerializeObject(e);
                
                output.Write(entry);
                
                output.Write("]}");
                if (count < logs.Count)
                    output.Write(",");
            }
            
            output.WriteLine("]}");
        }

        public void Format(IEnumerable<string> logEvents, TextWriter output)
        {
            throw new NotImplementedException();
        }

        private static void AddEventPropertyAsLabel(TextWriter output, string eventPropertyKey, LogEventPropertyValue eventPropertyValue)
        {
            output.Write(eventPropertyKey);
            output.Write("=\\\"");
            output.Write(eventPropertyValue.ToString());
            output.Write("\\\"");
        }

        private static void AddLabel(TextWriter output, string key, string value)
        {
            output.Write(key);
            output.Write("=\\\"");
            output.Write(value);
            output.Write("\\\"");
        }

        private static string GetLevel(LogEventLevel level)
        {
            if (level == LogEventLevel.Information)
                return "info";

            var r = level.ToString().ToLower();
            return r;
        }
    }
}