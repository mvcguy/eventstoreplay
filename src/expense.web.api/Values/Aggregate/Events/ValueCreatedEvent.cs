using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events
{
    public class ValueCreatedEvent : EventBase
    {
        public ValueCreatedEvent()
        {

        }

        public ValueCreatedEvent(IValueAggregateModel model) : base(model, ValueAggregateConstants.EventTypes.ValueCreated, 
            typeof(ValueCreatedEvent).AssemblyQualifiedName)
        {
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}