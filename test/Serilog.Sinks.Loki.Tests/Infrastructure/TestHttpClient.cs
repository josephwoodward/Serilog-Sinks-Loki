using System.Net.Http;
using System.Threading.Tasks;

namespace Serilog.Sinks.Loki.Tests.Infrastructure
{
    public class TestHttpClient : LokiHttpClient
    {
        public override async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            Content = await content.ReadAsStringAsync();
            RequestUri = requestUri;

            return new HttpResponseMessage();
        }

        public HttpClient Client => HttpClient;

        public string Content;

        public string RequestUri;
    }
}