using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events.Base;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events.Root
{
    public class ValueCreatedEvent : EventBase
    {
        public ValueCreatedEvent()
        {

        }

        public ValueCreatedEvent(IValuesRootAggregateModel model) : base(model, ValueAggregateConstants.EventTypes.ValueCreated, 
            typeof(ValueCreatedEvent).AssemblyQualifiedName)
        {
            Code = model.Code;
            Name = model.Name;
            Value = model.Value;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}