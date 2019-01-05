using System.Collections.Generic;

namespace Serilog.Sinks.Loki.Labels
{
    public interface ILabelProvider
    {
        IList<LokiLabel> GetLabels();
    }
}