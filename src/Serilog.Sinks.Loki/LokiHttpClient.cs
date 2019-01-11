using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Serilog.Sinks.Http;

namespace Serilog.Sinks.Loki
{
    public class LokiHttpClient : IHttpClient
    {
        protected readonly HttpClient HttpClient;

        public LokiHttpClient(HttpClient httpClient = null)
        {
            HttpClient = httpClient ?? new HttpClient();
            HttpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetAuthCredentials(LokiCredentials credentials)
        {
            if (!(credentials is BasicAuthCredentials c))
                return;

            var headers = HttpClient.DefaultRequestHeaders;
            if (headers.All(x => x.Key != "Authorization"))
            {
                var token = Base64Encode($"{c.Username}:{c.Password}");
                headers.Add("Authorization", $"Basic {token}");
            }
        }

        public virtual Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return HttpClient.PostAsync(requestUri, content);
        }

        public virtual void Dispose()
            => HttpClient.Dispose();

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}