namespace Serilog.Sinks.Loki
{
    using System.Collections.Generic;
    using System.Text;
    using Labels;
    using Newtonsoft.Json;

    internal class LokiContentStream
    {
        [JsonIgnore]
        public List<LokiLabel> Labels { get; } = new List<LokiLabel>();
            
        [JsonProperty("labels")]
        public string LabelsString {
            get
            {
                StringBuilder sb = new StringBuilder("{");
                bool firstLabel = true;
                foreach (LokiLabel label in Labels)
                {
                    if (firstLabel)
                        firstLabel = false;
                    else
                        sb.Append(",");

                    sb.Append(label.Key);
                    sb.Append("=\"");
                    sb.Append(label.Value);
                    sb.Append("\"");
                }
        
                sb.Append("}");
                return sb.ToString();
            } 
        }
            
        
        [JsonProperty("entries")]
        public List<LokiEntry> Entries { get; set; } = new List<LokiEntry>();
    }
}