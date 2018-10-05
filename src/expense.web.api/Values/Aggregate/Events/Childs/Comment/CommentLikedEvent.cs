using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events.Base;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events.Childs.Comment
{
    public class CommentLikedEvent : EventBase
    {
        public CommentLikedEvent()
        {
            
        }

        public CommentLikedEvent(IValueCommentChildAggregateDataModel model)
            : base(model,
                CommentAggConstants.EventType.CommentLiked,
                typeof(CommentLikedEvent).AssemblyQualifiedName)
        { }
    }
}