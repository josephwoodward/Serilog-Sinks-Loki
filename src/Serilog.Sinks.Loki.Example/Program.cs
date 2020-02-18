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
            NoAuthCredentials credentials = new NoAuthCredentials("http://localhost:3100");
            Logger log = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("MyPropertyName","MyPropertyValue")
                        .Enrich.WithThreadId()
                        .WriteTo.Console()
                        .WriteTo.LokiHttp(credentials, new LogLabelProvider(), new LokiExampleHttpClient())
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

            log.Dispose();
        }
    }
}