using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events.Base;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events.Root
{
    public class CodeChangedEvent : EventBase
    {
        public string Code { get; set; }

        public CodeChangedEvent()
        {

        }

        public CodeChangedEvent(IValuesRootAggregateModel model) : base(model, 
            ValueAggregateConstants.EventTypes.CodeChanged, 
            typeof(CodeChangedEvent).AssemblyQualifiedName)
        {
            Code = model.Code;
        }
    }
}