using expense.web.api.Values.Aggregate.Events;

namespace expense.web.api.Values.Aggregate.Constants
{
    public class ValueAggregateConstants
    {
        public class EventTypes
        {
            public const string ValueCreated = nameof(ValueCreatedEvent);
            public const string CodeChanged = nameof(CodeChangedEvent);
            public const string ValueChanged = nameof(ValueChangedEvent);
            public const string NameChanged = nameof(NameChangedEvent);
        }

        public class EventMetaDataHeaders
        {
            public const string EventClrTypeHeader = "EventClrTypeHeader";
            public const string AggregateClrTypeHeader = "AggregateClrTypeHeader";
            public const string CommitIdHeader = "CommitIdHeader";

        }
    }
}