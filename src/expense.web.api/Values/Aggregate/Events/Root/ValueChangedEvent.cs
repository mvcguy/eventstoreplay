using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events.Base;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events.Root
{
    public class ValueChangedEvent : EventBase
    {
        public string Value { get; set; }

        public ValueChangedEvent()
        {

        }

        public ValueChangedEvent(IValuesRootAggregateDataModel model) : base(model, ValueAggregateConstants.EventTypes.ValueChanged,
            typeof(ValueChangedEvent).AssemblyQualifiedName)
        {
            Value = model.Value;
        }
    }
}