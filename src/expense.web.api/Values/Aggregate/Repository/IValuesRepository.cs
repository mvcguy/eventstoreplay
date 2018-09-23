using System;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Repository
{
    public interface IValuesRepository : IRepository<ValuesAggregate>
    {
        /// <summary>
        /// Returns true if the record exists.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Exists(IValueAggregateModel model);

        /// <summary>
        /// Returns true if the record exists, in this case the out parameter is updated to the exisiting record id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="aggregateId"></param>
        /// <returns></returns>
        bool Exists(IValueAggregateModel model, out Guid aggregateId);

    }
}