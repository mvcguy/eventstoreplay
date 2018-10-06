using System;

namespace expense.web.api.Values.Aggregate.Model
{
    public interface IAggregateModel
    {
        Guid Id { get; }

        int TenantId { get; }

        Guid CommitId { get; }
    }

}
