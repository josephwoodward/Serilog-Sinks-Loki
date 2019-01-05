using System;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Loki.Enricher
{
    public class CorrelationIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null) throw new ArgumentNullException("logEvent");

            var correlationId = "";
            
            if (string.IsNullOrWhiteSpace(correlationId))
                return;

            var correlationIdProperty = new LogEventProperty("CorrelationId", new ScalarValue(correlationId));
            logEvent.AddPropertyIfAbsent(correlationIdProperty);
        }
    }
}