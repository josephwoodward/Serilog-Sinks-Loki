namespace Serilog.Sinks.Loki
{
  public class DefaultLokiHttpClient : LokiHttpClient
  {
    public DefaultLokiHttpClient(HttpClient httpClient = null) : base(httpClient) {}
  }
}
