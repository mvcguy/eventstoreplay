﻿using System;
using expense.web.api.Values.Aggregate.Events.Childs.Comment;
using expense.web.api.Values.Aggregate.Model;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.eventstore.EventStoreDataContext;

namespace expense.web.api.Values.Aggregate
{
    public class ValueCommentChildAggregate : AggregateBase<EventModel>, IValueCommentChildAggregateDataModel
    {
        private readonly IRepository<ValueCommentChildAggregate> _repository;

        /// <summary>
        /// root aggregate Id
        /// </summary>
        public Guid ParentId { get; }

        public string UserName { get; private set; }

        public string Comment { get; private set; }

        public int Likes { get; private set; }

        public int Dislikes { get; private set; }

        public Guid CommitId { get; set; }

        public int TenantId { get; set; }

        public ValueCommentChildAggregate()
        {

        }

        public ValueCommentChildAggregate(Guid parentId,
            IRepository<ValueCommentChildAggregate> repository,
            Guid? id = null, long version = -1)
        {
            _repository = repository;
            this.ParentId = parentId;
            this.Version = version;
            this.CommitId = Guid.NewGuid();
            this.Id = id ?? Guid.NewGuid();
        }

        public void AddComment(ValueCommentChildAggregateDataModel model)
        {
            ChangeCommentText(model.Comment, applyEvent: false);
            ChangeCommentUser(model.UserName, applyEvent: false);

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

            // Are we updating comment user except when the comment is first time created?
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
                    base.ApplyEvent(new CommentAddedEvent(this));
                    break;
                case CommentEventTypes.CommentTextChanged:
                    base.ApplyEvent(new CommentTextChangedEvent(this));
                    break;
                case CommentEventTypes.CommentLiked:
                    base.ApplyEvent(new CommentLikedEvent(this));
                    break;
                case CommentEventTypes.CommentDisliked:
                    base.ApplyEvent(new CommentDislikedEvent(this));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }
    }
}