using System.Collections.Generic;
using System.Linq;

namespace Serilog.Sinks.Loki.Labels
{
    public class DefaultLogLabelProvider : ILogLabelProvider
    {
        public DefaultLogLabelProvider() : this(null)
        {
        }

        public DefaultLogLabelProvider(IEnumerable<LokiLabel> labels,
            IEnumerable<string> propertiesAsLabels = null,
            IEnumerable<string> propertiesToAppend = null,
            LokiFormatterStrategy formatterStrategy = LokiFormatterStrategy.SpecificPropertiesAsLabelsAndRestAppended)
        {
            this.Labels = labels?.ToList() ?? new List<LokiLabel>();
            this.PropertiesAsLabels = propertiesAsLabels?.ToList() ?? new List<string> {"level"};
            this.PropertiesToAppend = propertiesToAppend?.ToList() ?? new List<string>();
            this.FormatterStrategy = formatterStrategy;
        }

        public IList<LokiLabel> GetLabels()
        {
            return this.Labels;
        }
        
        private IList<LokiLabel> Labels { get; }
        public IList<string> PropertiesAsLabels { get; }
        public IList<string> PropertiesToAppend { get; }
        public LokiFormatterStrategy FormatterStrategy { get; }
    }
}