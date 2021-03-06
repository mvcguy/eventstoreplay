﻿using System;
using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events.Base;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events.Childs.Comment
{
    public class CommentAddedEvent : EventBase
    {

        public string UserName { get; set; }

        public int Dislikes { get; set; }

        public int Likes { get; set; }

        public string CommentText { get; set; }

        public Guid ParentId { get; set; }

        public CommentAddedEvent()
        {

        }

        public CommentAddedEvent(IValueCommentAggregateChildDataModel model)
            : base(model,
                CommentAggConstants.EventType.CommentAdded,
                typeof(CommentAddedEvent).AssemblyQualifiedName)
        {
            this.ParentId = model.ParentId;
            this.CommentText = model.CommentText;
            this.Likes = model.Likes;
            this.Dislikes = model.Dislikes;
            this.UserName = model.UserName;
        }

    }
}
