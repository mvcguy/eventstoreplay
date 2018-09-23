using System;
using System.Collections.Generic;

namespace expense.web.api.Values.Aggregate.Model
{
    public interface IValueAggregateModel
    {
        int TenantId { get; }

        string Code { get; }

        string Name { get; }

        string Value { get; }

        Guid Id { get; }

        long Version { get; }

        Guid CommitId { get; }

        IList<ValueComment> Comments { get; }
    }

    public class ValueComment
    {
        public Guid CommentId { get; set; }

        public string UserName { get; set; }

        public string Comment { get; set; }
    }
}