using System;
using Serilog.Context;
using Serilog.Core;
using System.Threading;

namespace Serilog.Sinks.Loki.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger log = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("MyLabelPropertyName","MyPropertyValue")
                        .Enrich.WithThreadId()
                        .WriteTo.Console()
                        .WriteTo.LokiHttp(() => new LokiSinkConfiguration {
                            LokiUrl = "http://localhost:3100",
                            LogLabelProvider = new LogLabelProvider(),
                            HttpClient = new LokiExampleHttpClient()
                        })
                        .CreateLogger();

            log.Verbose("Verbose Text");

            int total = 3;
            for (int i = 1; i < total + 1; i++)
            {
                log.Debug("Processing item {ItemIndex} of {TotalItems}", i, total);
                Thread.Sleep(1000);
            }

            try
            {
                string invalidCast = (string) new object();
            }
            catch (Exception e)
            {
                log.Error(e, "Exception due to invalid cast");
            }

            var position = new { Latitude = 25, Longitude = 134 };
            log.Information("3# Random message processed {@Position} in {Elapsed:000} ms.", position, 34);

            using (LogContext.PushProperty("A", 1))
            {
                log.Warning("Warning with Property A");
                log.Fatal("Fatal with Property A");
            }

            using (LogContext.PushProperty("MyAppendPropertyName", 1))
            {
                log.Warning("Warning with Property MyAppendPropertyName");
                log.Fatal("Fatal with Property MyAppendPropertyName");
            }

            log.Dispose();
        }
    }
}