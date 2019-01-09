using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Serilog.Sinks.Http;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki
{
    public class TestLabels : ILogLabelProvider
    {
        public IList<LokiLabel> GetLabels()
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class LokiHttpClient : IHttpClient
    {
        protected readonly HttpClient HttpClient;

        public LokiHttpClient() : this(null)
        {
        }
        
        public LokiHttpClient(HttpClient httpClient)
        {
            HttpClient = httpClient ?? new HttpClient();
        }

        public virtual async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json"); 
            return await HttpClient.PostAsync(requestUri, content);
        }
        
        public virtual void Dispose()
            => HttpClient.Dispose();
    }
}