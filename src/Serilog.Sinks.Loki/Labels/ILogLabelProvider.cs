using System.Collections.Generic;

namespace Serilog.Sinks.Loki.Labels
{
    public interface ILogLabelProvider
    {
        IList<LokiLabel> GetLabels();
        bool PropertiesAsLabels { get; set; }
    }
}