using expense.web.api.Values.Aggregate.Events.Childs.Comment;

namespace expense.web.api.Values.Aggregate.Constants
{
    public class CommentAggConstants
    {
        public class EventType
        {
            public const string CommentAdded = nameof(CommentAddedEvent);
            public const string CommentLiked = nameof(CommentAddedEvent);
            public const string CommentDisliked = nameof(CommentDislikedEvent);
            public const string CommentTextChanged = nameof(CommentTextChangedEvent);
        }
    }
}
