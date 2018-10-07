using System;
using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events.Base;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events.Childs.Comment
{
    public class CommentTextChangedEvent : EventBase
    {
        public string CommentText { get; set; }

        public Guid ParentId { get; set; }

        public CommentTextChangedEvent()
        {
            
        }

        public CommentTextChangedEvent(IValueCommentAggregateChildDataModel model)
            : base(model,
                CommentAggConstants.EventType.CommentTextChanged,
                typeof(CommentTextChangedEvent).AssemblyQualifiedName)
        {
            this.CommentText = model.CommentText;
            this.ParentId = model.ParentId;
        }
    }
}