using System.Net.Http;
using System.Threading.Tasks;

namespace Serilog.Sinks.Loki.Example
{
    public class LokiExampleHttpClient : LokiHttpClient
    {
        public override async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            var r = content.ReadAsStringAsync().Result;

            var result = await base.PostAsync(requestUri, content);
            var body = result.Content.ReadAsStringAsync().Result; //right!

            return result;
        }
    }
}