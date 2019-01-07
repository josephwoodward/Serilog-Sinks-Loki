# Serilog.Sinks.Loki

This is a Serilog Sink for Grafana's new [Loki Log Aggregator](https://github.com/grafana/loki).

Note: It's still really early work in progress.

## Current Features:

- Formats and batches log entries to Loki via HTTP
- Ability to provide global Loki log labels

Coming soon:

- Ability to provide contextual log labels
- Write logs to disk in the correct format to send via Promtail
- Send logs to Loki via HTTP using Snappy compression

## Usage Example:

```csharp

var log = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.FromLogContext()
        .WriteTo.LokiHttp("http://localhost:3100/api/prom/push", new CustomLogLabelProvider())
        .CreateLogger();

var exception = new {Message = ex.Message, StackTrace = ex.StackTrace};
log.Error(exception);

var position = new { Latitude = 25, Longitude = 134 };
var elapsedMs = 34;
log.Information("Message processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);
```

### Adding global labels

Global labels can be added by implementing the `ILogLabelProvider` class:

```csharp
public class LogLabelProvider : ILogLabelProvider
        {
        public IList<LokiLabel> GetLabels()
        {
            return new List<LokiLabel>
            {
                new LokiLabel { Key = "app", Value = "demoapp" },
                new LokiLabel { Key = "environment", Value = "production" }
            };
        }
}

```
