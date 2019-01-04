using System.Collections.Generic;

namespace Serilog.Sinks.Loki
{
    public class LokiRequest
    {
        public IList<LokiStream> Streams { get; set; }
    }

    public class LokiStream
    {
        
    }
}