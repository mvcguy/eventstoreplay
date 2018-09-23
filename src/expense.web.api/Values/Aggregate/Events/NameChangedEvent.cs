using System.Collections.Generic;
using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events
{
    public class NameChangedEvent : EventBase
    {
        public string Name { get; set; }

        public NameChangedEvent()
        {

        }

        public NameChangedEvent(IValueAggregateModel model) : base(model, ValueAggregateConstants.EventTypes.NameChanged,
            typeof(NameChangedEvent).AssemblyQualifiedName)
        {

        }
    }
}