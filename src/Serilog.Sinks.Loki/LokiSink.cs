using System;
using System.Net.Http;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Http.Private.Sinks;

namespace Serilog.Sinks.Loki
{
    public class LokiSink : ILogEventSink
    {
        private readonly HttpSink _baseAddress;
        private readonly IFormatProvider _formatProvider;

        public LokiSink(HttpSink baseAddress, IFormatProvider formatProvider)
        {
            _baseAddress = baseAddress;
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            _baseAddress.Emit(logEvent);
            // 2018-12-19T09:07:16Z docker time="2018-12-19T09:07:16.074537600Z" level=debug msg="[resolver] lookup name loki. present without IPv6 address"
            // 2018-12-19T09:25:30Z 
            var b = new StringBuilder();
            
            var utc = logEvent.Timestamp.UtcDateTime;
            var time = utc.ToString("yyyy-MM-ddTHH:mm:ssK");
            
            b.Append(time);
            b.Append(" ");
            b.Append("component");
            b.Append(" ");
            b.Append($"time=\"{utc:o}\"");
            b.Append(" ");
            b.Append($"level={logEvent.Level}");
            b.Append(" ");
            
            var message = logEvent.RenderMessage(_formatProvider);
            
            Console.WriteLine(b.ToString());
        }
    }
}