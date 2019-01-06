using System.Collections.Generic;

namespace Serilog.Sinks.Loki.Tests
{
    public class TestResponse
    {
        public IList<Stream> Streams { get; set; }
    }

    public class Stream
    {
        public string Labels { get; set; }
    }
}