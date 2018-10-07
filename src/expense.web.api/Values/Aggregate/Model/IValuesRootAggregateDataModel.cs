using System;
using System.Collections.Generic;

namespace expense.web.api.Values.Aggregate.Model
{
    // root aggregate
    public interface IValuesRootAggregateDataModel : IAggregateModel
    {
        string Code { get; }

        string Name { get; }

        string Value { get; }

        long Version { get; }

        IList<ValueCommentAggregateChild> Comments { get; }
    }

}