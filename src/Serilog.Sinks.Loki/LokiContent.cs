namespace Serilog.Sinks.Loki
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    internal class LokiContent
    {
        [JsonProperty("streams")]
        public List<LokiContentStream> Streams { get; set; } = new List<LokiContentStream>();

        public string Serialize()
        {
            JsonSerializer serializer = new JsonSerializer();
            TextWriter writer = new StringWriter();
            serializer.Serialize(writer, this);
            return writer.ToString();
        }
    }
}