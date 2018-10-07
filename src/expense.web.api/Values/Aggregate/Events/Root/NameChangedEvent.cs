using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events.Base;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events.Root
{
    public class NameChangedEvent : EventBase
    {
        public string Name { get; set; }

        public NameChangedEvent()
        {

        }

        public NameChangedEvent(IValuesRootAggregateDataModel model) : base(model, ValueAggregateConstants.EventTypes.NameChanged,
            typeof(NameChangedEvent).AssemblyQualifiedName)
        {
            Name = model.Name;
        }
    }
}