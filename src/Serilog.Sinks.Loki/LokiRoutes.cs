namespace Serilog.Sinks.Loki
{
    public static class LokiRouteBuilder
    {
        public static string BuildPostUri(string host)
        {
            return host.Substring(host.Length - 1) != "/" ? $"{host}{PostDataUri}" : $"{host.TrimEnd('/')}{PostDataUri}";
        }

        public const string PostDataUri = "/api/prom/push";
    }
}