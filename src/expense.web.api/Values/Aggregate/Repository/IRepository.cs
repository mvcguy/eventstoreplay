using System;

namespace expense.web.api.Values.Aggregate.Repository
{
    public interface IRepository<TAggregate> where TAggregate : class, IAggregate
    {
        TAggregate GetById(Guid id);

        TAggregate GetById(Guid id, long version);

        bool Save(TAggregate aggregate);

        /// <summary>
        /// Returns true if the record exists.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Exists(TAggregate model);

        /// <summary>
        /// Returns true if the record exists, in this case the out parameter is updated to the existing record id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="aggregateId"></param>
        /// <returns></returns>
        bool Exists(TAggregate model, out Guid aggregateId);
    }
}