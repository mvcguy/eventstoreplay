using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events
{
    public class CodeChangedEvent : EventBase
    {
        public string Code { get; set; }

        public CodeChangedEvent()
        {

        }

        public CodeChangedEvent(IValueAggregateModel model) : base(model, 
            ValueAggregateConstants.EventTypes.CodeChanged, 
            typeof(CodeChangedEvent).AssemblyQualifiedName)
        {

        }
    }
}