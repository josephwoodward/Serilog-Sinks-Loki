namespace Serilog.Sinks.Loki.Labels
{
    public class LokiLabel
    {
        public LokiLabel(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public string Value { get; }
    }
}