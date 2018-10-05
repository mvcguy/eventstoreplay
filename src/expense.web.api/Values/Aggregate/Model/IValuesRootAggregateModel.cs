namespace expense.web.api.Values.Aggregate.Model
{
    // root aggregate
    public interface IValuesRootAggregateModel : IAggregateModel
    {
        string Code { get; }

        string Name { get; }

        string Value { get; }
    }

}