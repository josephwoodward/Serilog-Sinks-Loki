using System.Collections.Generic;

namespace Serilog.Sinks.Loki.Tests.Infrastructure
{
    public class TestResponse
    {
        public TestResponse()
        {
            Streams = new List<Stream>();
        }
        
        public IList<Stream> Streams { get; set; }
    }

    public class Stream
    {
        public string Labels { get; set; }
        public List<Entry> Entries { get; set; }
    }

    public class Entry
    {
        public string Line { get; set; }
    }
}