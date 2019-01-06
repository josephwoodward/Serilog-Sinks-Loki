using System.Net.Http;
using System.Threading.Tasks;

namespace Serilog.Sinks.Loki.Tests
{
    public class TestHttpClient : LokiHttpClient
    {
        public override Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            Content = content.ReadAsStringAsync().Result;
            RequestUri = requestUri;

            return Task.FromResult(new HttpResponseMessage());
        }

        public string Content;

        public string RequestUri;

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}