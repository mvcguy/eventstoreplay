using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Commands.Value
{
    public class ValueCommandResponse : CommandResponseBase
    {
        public IValuesRootAggregateModel Model { get; set; }
    }
}