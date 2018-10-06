﻿using System;

namespace expense.web.api.Values.Aggregate.Model
{
    public interface IValueCommentAggregateChildDataModel : IAggregateModel
    {
        Guid ParentId { get; }

        string UserName { get; }

        string Comment { get; }

        int Likes { get; }

        int Dislikes { get; }


    }
}