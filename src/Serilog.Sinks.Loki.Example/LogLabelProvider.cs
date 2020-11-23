using System.Collections.Generic;
using Serilog.Sinks.Loki.Labels;

namespace Serilog.Sinks.Loki.Example
{
    public class LogLabelProvider : ILogLabelProvider
    {
        public IList<LokiLabel> GetLabels()
        {
            return new List<LokiLabel>
            {
                new LokiLabel("app", "demo"),
                new LokiLabel("namespace", "prod")
            };
        }

        public IList<string> PropertiesAsLabels { get; set; } = new List<string>
        {
            "MyLabelPropertyName"
        };
        public IList<string> PropertiesToAppend { get; set; } = new List<string>
        {
            "MyAppendPropertyName"
        };
        public LokiFormatterStrategy FormatterStrategy { get; set; } = LokiFormatterStrategy.SpecificPropertiesAsLabelsOrAppended;
    }
}