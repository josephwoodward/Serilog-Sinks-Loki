using Newtonsoft.Json;

namespace Serilog.Sinks.Loki
{
    internal class LokiEntry 
    {
        public LokiEntry(string ts, string line)
        {
            Ts = ts;
            Line = line;
        }

        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("line")]
        public string Line { get; set; }
    }
}