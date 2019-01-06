using System.Collections.Generic;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki.Tests
{
    public class TestLabelProvider : ILogLabelProvider
    {
        public IList<LokiLabel> GetLabels()
        {
            return new List<LokiLabel>
            {
                new LokiLabel
                {
                    Key = "app",
                    Value = "tests"
                }
            };
        }
    }
}