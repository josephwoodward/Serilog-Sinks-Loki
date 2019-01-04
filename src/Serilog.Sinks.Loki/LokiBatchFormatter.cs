using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.Http;
using Serilog.Sinks.Http.BatchFormatters;

namespace Serilog.Sinks.Loki
{
    public class LokiEntry 
    {
        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("line")]
        public string Line { get; set; }
    }
    
    public class LokiBatchFormatter : IBatchFormatter 
    {
        public void Format(IEnumerable<LogEvent> logEvents, ITextFormatter formatter, TextWriter output)
        {
            if (logEvents == null)
                throw new ArgumentNullException(nameof (logEvents));
            if (output == null)
                throw new ArgumentNullException(nameof (output));
            
            var logs = logEvents.ToList();
            if (!logs.Any())
                return;

            var count = 0;
            
            output.Write("{ \"streams\": [");
            foreach (var logEvent in logs)
            {
                count++;
                output.Write("{");
                
                var localTime = DateTime.Now;
                var localTimeAndOffset = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));
                
                var time = localTimeAndOffset.ToString("o");
                
                /*output.Write("\"labels\": \"{Level=\\\"" + logEvent.Level + "\\\"}\",")*/;
                output.Write("\"labels\": \"{");
                
                AddLabel(output, "level", logEvent.Level.ToString());

/*                if (logEvent.Properties.Any())
                {
                    foreach (var eventProperty in logEvent.Properties)
                    {
                        output.Write(",");
                        AddEventPropertyAsLabel(output, eventProperty.Key, eventProperty.Value);
                    } 
                }*/
                
                output.Write("}\",");
                output.Write("\"entries\":[");

                var entry = JsonConvert.SerializeObject( new LokiEntry
                {
                    Ts = time,
                    Line = logEvent.RenderMessage()
                });
                
                output.Write(entry);
                
                output.Write("]");
                output.Write("}");
                if (count < logs.Count)
                    output.Write(",");
            }
            
            output.Write("]}");
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

        public void Format(IEnumerable<string> logEvents, TextWriter output)
        {
            var res = 1;
        }
    }
}