using System;
using System.Collections.Generic;

namespace expense.web.eventstore.EventStoreDataContext
{
    public interface IEsAggregate<TEventModel> where TEventModel : IEventModel
    {
        long Version { get; set; }

        Guid Id { get; set; }

        IList<TEventModel> Events { get; set; }
    }
}