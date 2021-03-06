﻿using System;
using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events.Base;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events.Childs.Comment
{
    public class CommentDislikedEvent : EventBase
    {
        public Guid ParentId { get; set; }
        public CommentDislikedEvent()
        {
            
        }

        public CommentDislikedEvent(IValueCommentAggregateChildDataModel model)
            : base(model,
                CommentAggConstants.EventType.CommentDisliked,
                typeof(CommentDislikedEvent).AssemblyQualifiedName)
        {
            this.ParentId = model.ParentId;
        }
    }
}