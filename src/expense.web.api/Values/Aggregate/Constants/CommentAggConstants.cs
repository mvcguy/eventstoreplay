using expense.web.api.Values.Aggregate.Events.Childs.Comment;

namespace expense.web.api.Values.Aggregate.Constants
{
    public class CommentAggConstants
    {
        public class EventType
        {
            // BUG: We need to use string literals, in case events are renamed, we would not be able to parse correctly old events!!!
            public const string CommentAdded = nameof(CommentAddedEvent);
            public const string CommentLiked = nameof(CommentLikedEvent);
            public const string CommentDisliked = nameof(CommentDislikedEvent);
            public const string CommentTextChanged = nameof(CommentTextChangedEvent);
        }
    }
}
