namespace Serilog.Sinks.Loki {
    public enum LokiFormatterStrategy {
        /// All Serilog Event properties will be sent as labels
        AllPropertiesAsLabels,

        /// Specific Serilog Event properties will be sent as labels.
        /// The rest of properties will be discarder.
        SpecificPropertiesAsLabelsAndRestDiscarded,

        /// Specific Serilog Event properties will be sent as labels.
        /// The rest of properties will be appended to the log message.
        SpecificPropertiesAsLabelsAndRestAppended,

        /// Specific Serilog Event properties will be sent as labels.
        /// Other specific properties will be appended to the log message.
        /// The rest of properties will be discarded
        SpecificPropertiesAsLabelsOrAppended
    }
}
