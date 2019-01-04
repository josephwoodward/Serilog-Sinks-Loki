using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.Http;
using Serilog.Sinks.Http.BatchFormatters;

namespace Serilog.Sinks.Loki.Example
{
    public class Client : IHttpClient
    {
        private readonly HttpClient _client;

        public Client()
        {
            _client = new HttpClient();
        }
        
        public void Dispose()
            => _client.Dispose();

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            var r = content.ReadAsStringAsync().Result;
            var response = await _client.PostAsync(requestUri, content);

            return response;
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var r = new DefaultBatchFormatter();
            
            Exception ex; 
            try
            {
                throw new Exception("Something went wrong, see StackTrace for more info");
            }
            catch (Exception e)
            {
                ex = e;
            }

            var l = new LokiBatchFormatter();
            
            var log = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                .WriteTo.Http("http://localhost:3100/api/prom/push", httpClient: new Client(), batchFormatter: l)
                .CreateLogger();

            using (LogContext.PushProperty("A", 1))
            {
                log.Information("Carries property A = 1");
            }
            
            log.Information("1# Logging {@Heartbeat:l} from {Computer:l}", "SomeValue", "SomeOtherValue");

            var position = new { Latitude = 25, Longitude = 134 };
            var elapsedMs = 34;

            log.Debug(@"Does this \""break\"" something?");
            log.Error("#2 {@Message}, {@StackTrace}", new { Message = ex.Message, StackTrace = ex.StackTrace});
            log.Information("3# Random message processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);
            
            log.Dispose();
        }
    }
}