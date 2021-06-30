# Serilog.Sinks.Loki

![.NET Core](https://github.com/JosephWoodward/Serilog-Sinks-Loki/workflows/.NET%20Core/badge.svg?branch=master)

This is a Serilog Sink for Grafana's new [Loki Log Aggregator](https://grafana.com/loki).

What is Loki?

> Loki is a horizontally-scalable, highly-available, multi-tenant log aggregation system inspired by Prometheus. It is designed to be very cost effective and easy to operate, as it does not index the contents of the logs, but rather a set of labels for each log stream.

You can find more information about what Loki is over on [Grafana's website here](https://grafana.com/loki).

![Loki Screenshot](https://raw.githubusercontent.com/JosephWoodward/Serilog-Sinks-Loki/master/assets/screenshot2.png)

## Current Features:

- Formats and batches log entries to Loki via HTTP
- Ability to provide global Loki log labels
- Comes baked with an HTTP client, but your own can be provided
- Provides contextual log labels

Coming soon:

- Write logs to disk in the correct format to send via Promtail
- Send logs to Loki via HTTP using Snappy compression

## Installation

The Serilog.Sinks.Loki NuGet [package can be found here](https://www.nuget.org/packages/Serilog.Sinks.Loki/). Alternatively you can install it via one of the following commands below:

NuGet command:
```bash
Install-Package Serilog.Sinks.Loki
```
.NET Core CLI:
```bash
dotnet add package Serilog.Sinks.Loki
```

## Basic Example:

```csharp
// var credentials = new BasicAuthCredentials("http://localhost:3100", "<username>", "<password>");
var credentials = new NoAuthCredentials("http://localhost:3100"); // Address to local or remote Loki server

Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.LokiHttp(credentials)
        .CreateLogger();

var exception = new {Message = ex.Message, StackTrace = ex.StackTrace};
Log.Error(exception);

var position = new { Latitude = 25, Longitude = 134 };
var elapsedMs = 34;
Log.Information("Message processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);

Log.CloseAndFlush();
```

### Adding global labels

Loki indexes and groups log streams using labels, in Serilog.Sinks.Loki you can attach labels to all log entries by passing an implementation `ILogLabelProvider` to the `WriteTo.LokiHttp(..)` configuration method. This is ideal for labels such as instance IDs, environments and application names.

__See [Serilog.Sinks.Loki.Example/LogLabelProvider.cs](https://github.com/JosephWoodward/Serilog-Sinks-Loki/blob/master/src/Serilog.Sinks.Loki.Example/LogLabelProvider.cs)__ for a basic `LogLabelProvider` implementation.

```csharp
var credentials = new BasicAuthCredentials("http://localhost:3100", "<username>", "<password>");
var log = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.FromLogContext()
        .WriteTo.LokiHttp(credentials, new LogLabelProvider())
        .CreateLogger();
```

### Local, contextual labels

In some occasions you'll want to add context to your log stream within a particular class or method, this can be achieved using contextual labels:

```csharp
using (LogContext.PushProperty("A", 1))
{
    log.Warning("Warning with Property A");
    log.Fatal("Fatal with Property A");
}
```

### Custom HTTP Client

Serilog.Loki.Sink is built on top of the popular [Serilog.Sinks.Http](https://github.com/FantasticFiasco/serilog-sinks-http) library to post log entries to Loki. With this in mind you can you can extend the default HttpClient (`LokiHttpClient`), or replace it entirely by implementing `IHttpClient`.

```csharp
// ExampleHttpClient.cs

public class ExampleHttpClient : LokiHttpClient
{
    public override Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
    {
        return base.PostAsync(requestUri, content);
    }
}
```
```csharp
// Usage

var credentials = new BasicAuthCredentials("http://localhost:3100", "<username>", "<password>");
var log = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.FromLogContext()
        .WriteTo.LokiHttp(credentials, new LogLabelProvider(), new ExampleHttpClient())
        .CreateLogger();
```

### Configure using appsettings.json
`Serilog-Sinks-Loki` can be configured with `appsettings.json` using `Serilog.Settings.Configuration`.  
It support the following arguments `serverUrl`, `username`, `password`, `credentials`, `labelProvider`, `httpClient`, `outputTemplate` and `formatProvider`.  
Not all fields can be used in combination look in [LokiSinkExtensions.cs](src/Serilog.Sinks.Loki/LokiSinkExtensions.cs) for the supported combinations.  
`credentials`, `labelProvider`, `httpClient`, and `formatProvider` are classes and must be specified using the `Namespace.ClassName, Assembly` syntax.
```json
"Serilog": {
  "Using": [ "Serilog.Sinks.Loki" ],
  "MinimumLevel": {
    "Default": "Verbose"
  },
  "Enrich": [ "FromLogContext" ],
  "WriteTo": [      
    {
      "Name": "LokiHttp",
      "Args": {
        "serverUrl": "https://loki:3000",
        "labelProvider": "Namespace.ClassName, Assembly"
      }
    }
  ]
}
```

### Missing a feature or want to contribute?
This package is still in its infancy so if there's anything missing then please feel free to raise a feature request, either that or pull requests are most welcome!
