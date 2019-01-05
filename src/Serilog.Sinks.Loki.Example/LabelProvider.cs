using System.Collections.Generic;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki.Example
{
    public class LabelProvider : ILabelProvider
    {
        public IList<LokiLabel> GetLabels()
        {
            return new List<LokiLabel>
            {
                new LokiLabel
                {
                    Key = "app",
                    Value = "demo"
                }
            };
        }
    }
}