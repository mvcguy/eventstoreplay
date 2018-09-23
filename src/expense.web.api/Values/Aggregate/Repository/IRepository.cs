using System;
using System.Collections.Generic;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Repository
{
    public interface IRepository<TAggregate> where TAggregate : class, IAggregate
    {
        TAggregate GetById(Guid id);

        TAggregate GetById(Guid id, long version);

        bool Save(TAggregate aggregate);
    }
}