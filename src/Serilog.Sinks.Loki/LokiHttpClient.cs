using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Serilog.Sinks.Http;

namespace Serilog.Sinks.Loki
{
    internal class LokiHttpClient : IHttpClient
    {
        private readonly HttpClient _client;

        public LokiHttpClient()
        {
            _client = new HttpClient();
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            var r = content.ReadAsStringAsync().Result;
            var response = await _client.PostAsync(requestUri, content);
            
            var body = response.Content.ReadAsStringAsync().Result; //right!

            return response;
        }
        
        public void Dispose()
            => _client.Dispose();
    }
}