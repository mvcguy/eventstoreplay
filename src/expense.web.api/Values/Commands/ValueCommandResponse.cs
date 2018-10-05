using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Commands
{
    public class ValueCommandResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public IValuesRootAggregateModel ValuesRootAggregateModel { get; set; }
    }
}