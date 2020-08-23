using System.Collections.Generic;

namespace Serilog.Sinks.Loki.Labels
{
    class DefaultLogLabelProvider : ILogLabelProvider
    {
        public IList<LokiLabel> GetLabels()
        {
            return new List<LokiLabel>();
        }

        public bool PropertiesAsLabels { get; set; } = true;
    }
}