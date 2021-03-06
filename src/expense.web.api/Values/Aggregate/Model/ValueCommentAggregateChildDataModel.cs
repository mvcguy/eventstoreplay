﻿using System;

namespace expense.web.api.Values.Aggregate.Model
{
    public class ValueCommentAggregateChildDataModel : IValueCommentAggregateChildDataModel
    {
        public long ParentVersion { get; set; }

        public Guid CommitId { get; set; }
        public Guid Id { get; set; }
        public int TenantId { get; set; }
        public Guid ParentId { get; set; }
        public string UserName { get; set; }
        public string CommentText { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}