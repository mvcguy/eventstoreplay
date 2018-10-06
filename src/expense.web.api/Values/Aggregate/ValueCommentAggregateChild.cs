using System;
using expense.web.api.Values.Aggregate.Events.Childs.Comment;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate
{
    public class ValueCommentAggregateChild : IValueCommentAggregateChildDataModel
    {
        private readonly ValuesRootAggregate _root;

        /// <summary>
        /// root aggregate Id
        /// </summary>
        public Guid ParentId { get; }

        public string UserName { get; private set; }

        public string Comment { get; private set; }

        public int Likes { get; private set; }

        public int Dislikes { get; private set; }

        public Guid Id { get; }

        public int TenantId { get; set; }
        public Guid CommitId { get; }

        public ValueCommentAggregateChild()
        {

        }

        public ValueCommentAggregateChild(ValuesRootAggregate root, Guid? id = null)
        {
            _root = root;
            this.ParentId = root.Id;
            this.CommitId = root.CommitId;
            this.Id = id ?? Guid.NewGuid();
        }

        public void AddComment(IValueCommentAggregateChildDataModel model, bool applyEvent = true)
        {
            // when a comment is first added, we don't need to fire individual events
            ChangeCommentText(model.Comment, applyEvent: false);
            ChangeCommentUser(model.UserName, applyEvent: false);

            if (!applyEvent) return;

            ApplyEvent(CommentEventTypes.CommentAdded);
        }

        public void ChangeCommentText(string text, bool applyEvent = true)
        {
            this.Comment = text;

            if (!applyEvent) return;

            ApplyEvent(CommentEventTypes.CommentTextChanged);
        }

        public void ChangeCommentUser(string user, bool applyEvent = true)
        {
            this.UserName = user;

            if (!applyEvent) return;

            // Are we updating username beside when the comment is first time created?
        }

        public void CommentLiked(bool applyEvent = true)
        {
            this.Likes += 1;

            if (!applyEvent) return;

            ApplyEvent(CommentEventTypes.CommentLiked);
        }

        public void CommendDisliked(bool applyEvent = true)
        {
            this.Dislikes += 1;

            if (!applyEvent) return;

            ApplyEvent(CommentEventTypes.CommentDisliked);
        }

        public void ApplyEvent(CommentEventTypes eventType)
        {
            switch (eventType)
            {
                case CommentEventTypes.CommentAdded:
                    _root.ApplyEvent(new CommentAddedEvent(this));
                    break;
                case CommentEventTypes.CommentTextChanged:
                    _root.ApplyEvent(new CommentTextChangedEvent(this));
                    break;
                case CommentEventTypes.CommentLiked:
                    _root.ApplyEvent(new CommentLikedEvent(this));
                    break;
                case CommentEventTypes.CommentDisliked:
                    _root.ApplyEvent(new CommentDislikedEvent(this));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }
    }
}