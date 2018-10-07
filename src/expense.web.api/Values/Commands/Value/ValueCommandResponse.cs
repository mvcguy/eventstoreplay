using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Commands.Value
{
    public class ValueCommandResponse : CommandResponseBase
    {
        public IValuesRootAggregateDataModel Model { get; set; }
    }
}