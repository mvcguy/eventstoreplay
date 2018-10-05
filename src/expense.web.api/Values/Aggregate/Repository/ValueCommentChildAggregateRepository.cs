using System;

namespace expense.web.api.Values.Aggregate.Repository
{
    public class ValueCommentChildAggregateRepository : IRepository<ValueCommentChildAggregate>
    {
        public ValueCommentChildAggregate GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public ValueCommentChildAggregate GetById(Guid id, long version)
        {
            throw new NotImplementedException();
        }

        public bool Save(ValueCommentChildAggregate aggregate)
        {
            throw new NotImplementedException();
        }

        public bool Exists(ValueCommentChildAggregate model)
        {
            throw new NotImplementedException();
        }

        public bool Exists(ValueCommentChildAggregate model, out Guid aggregateId)
        {
            throw new NotImplementedException();
        }
    }
}