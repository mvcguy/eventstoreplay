using System;

namespace expense.web.eventstore.EventStoreDataContext
{
    public class AggregateVersionException : Exception
    {
        public AggregateVersionException(Guid id, Type type)
            : base($"Version mismatch error: Aggregate: {type}, ID: {id}")
        {

        }
    }
}