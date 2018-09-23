using System;
using System.Collections;

namespace expense.web.api.Values.Aggregate
{
    public interface IAggregate
    {
        Guid Id { get; }

        long Version { get; }

        void ApplyEvent(object @event);
    }
}