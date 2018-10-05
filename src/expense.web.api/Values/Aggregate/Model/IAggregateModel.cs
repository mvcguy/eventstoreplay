using System;

namespace expense.web.api.Values.Aggregate.Model
{
    public interface IAggregateModel
    {
        long Version { get; }

        Guid CommitId { get; }

        Guid Id { get; }

        int TenantId { get; }
    }
}
