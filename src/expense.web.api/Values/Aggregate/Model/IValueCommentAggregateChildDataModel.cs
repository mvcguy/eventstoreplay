using System;

namespace expense.web.api.Values.Aggregate.Model
{
    public interface IValueCommentAggregateChildDataModel : IAggregateModel
    {
        Guid ParentId { get; }

        long ParentVersion { get; }
        
        string UserName { get; }

        string CommentText { get; }

        int Likes { get; }

        int Dislikes { get; }


    }
}