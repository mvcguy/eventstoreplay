﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events.Base;
using expense.web.api.Values.Aggregate.Events.Childs.Comment;
using expense.web.api.Values.Aggregate.Events.Root;
using expense.web.api.Values.ReadModel.Schema;
using expense.web.eventstore.EventStoreDataContext;
using expense.web.eventstore.EventStoreSubscriber;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace expense.web.api.Values.ReadModel
{
    public class ValuesEventConsumer : EventStoreSubscriberBase
    {
        private readonly IMongoClient _mongoClient;
        private readonly ILogger<ValuesEventConsumer> _logger;
        private readonly IReadModelRepository<ValueRecord> _valueRecordsRepository;
        private readonly IReadModelRepository<ReadPointer> _readPointerRepository;
        private ValueRecord _currentValueRecord;
        private DatabaseOperation _operation;
        private readonly ReadPointer _readPointer;

        public ValuesEventConsumer(IOptions<SubscriberOptions> options,
            IMongoClient mongoClient,
            ILogger<ValuesEventConsumer> logger,
            IReadModelRepository<ValueRecord> valueRecordsRepository,
            IReadModelRepository<ReadPointer> readPointerRepository,
            IEventStoreConnection eventStoreConnection) : base(options, eventStoreConnection)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _valueRecordsRepository = valueRecordsRepository;
            _readPointerRepository = readPointerRepository;

            // there must be at least one record!!!
            _readPointer = _readPointerRepository.GetAll()
                .First(x => x.SourceName == options.Value.TopicName);
        }

        public override void OnEvent(EventModel @event)
        {

            var metaDataJson = Encoding.UTF8.GetString(@event.Metadata);
            var eventDataJson = Encoding.UTF8.GetString(@event.Data);

            var eventMetaData = JsonConvert.DeserializeObject<EventMetaData>(metaDataJson);

            _operation = DatabaseOperation.Undefined;
            switch (@event.EventType)
            {
                case ValueAggregateConstants.EventTypes.ValueCreated:
                    Handle(JsonConvert.DeserializeObject<ValueCreatedEvent>(eventDataJson), eventDataJson, @event, eventMetaData);
                    break;
                case ValueAggregateConstants.EventTypes.NameChanged:
                    Handle(JsonConvert.DeserializeObject<NameChangedEvent>(eventDataJson), eventDataJson, @event, eventMetaData);
                    break;
                case ValueAggregateConstants.EventTypes.CodeChanged:
                    Handle(JsonConvert.DeserializeObject<CodeChangedEvent>(eventDataJson), eventDataJson, @event, eventMetaData);
                    break;
                case ValueAggregateConstants.EventTypes.ValueChanged:
                    Handle(JsonConvert.DeserializeObject<ValueChangedEvent>(eventDataJson), eventDataJson, @event, eventMetaData);
                    break;

                // comments (aggregate child)
                case CommentAggConstants.EventType.CommentAdded:
                    Handle(JsonConvert.DeserializeObject<CommentAddedEvent>(eventDataJson), eventDataJson, @event, eventMetaData);
                    break;
                case CommentAggConstants.EventType.CommentTextChanged:
                    Handle(JsonConvert.DeserializeObject<CommentTextChangedEvent>(eventDataJson), eventDataJson, @event, eventMetaData);
                    break;
                case CommentAggConstants.EventType.CommentLiked:
                    Handle(JsonConvert.DeserializeObject<CommentLikedEvent>(eventDataJson), eventDataJson, @event, eventMetaData);
                    break;
                case CommentAggConstants.EventType.CommentDisliked:
                    Handle(JsonConvert.DeserializeObject<CommentDislikedEvent>(eventDataJson), eventDataJson, @event, eventMetaData);
                    break;
                default:
                    _logger.LogInformation($"Unknown event type: {@event.EventType}, Payload: {eventDataJson}");
                    break;
            }

            // TODO: Check that the event version is higher than the current stored record


            if (_operation == DatabaseOperation.Undefined)
            {
                _logger.LogError($"Undefined operation. @Event: {eventDataJson}");
                return;
            };
            using (var session = _mongoClient.StartSession())
            {
                try
                {
                    session.StartTransaction();
                    switch (_operation)
                    {
                        case DatabaseOperation.Create:
                            InsertRecord(@event, session);
                            UpdateReadPointer(@event.SequenceNumber, session);
                            break;
                        case DatabaseOperation.Update:
                            UpdateRecord(@event, session);
                            UpdateReadPointer(@event.SequenceNumber, session);
                            break;
                        case DatabaseOperation.Delete:
                            DeleteRecord(@event, session);
                            UpdateReadPointer(@event.SequenceNumber, session);
                            break;
                    }
                    ThrowIfThereIsHole(@event, _currentValueRecord.PublicId.GetValueOrDefault());
                    session.CommitTransaction();
                    _logger.LogInformation($"Event {@event.EventType} is successfully handled and persisted to database");

                }
                catch (Exception e)
                {
                    _logger.LogError($"Error: Event {@event.EventType} cannot be handled.");
                    _logger.LogError(e.Message, e);
                    session.AbortTransaction();
                }
            }

        }

        private void Handle(CommentDislikedEvent commentDislikedEvent, string eventDataJson, EventModel @event, EventMetaData eventMetaData)
        {
            _logger.LogInformation($"Handling {nameof(CommentDislikedEvent)} .... {Environment.NewLine} {eventDataJson}");

            _operation = DatabaseOperation.Update;
            _currentValueRecord = GetRecord(commentDislikedEvent.ParentId);

            ThrowIfRecordIsNull(commentDislikedEvent, _currentValueRecord);

            // there must exist a comment record already before we can update it!
            var comment = _currentValueRecord.Comments.First(x => x.PublicId == commentDislikedEvent.Id);

            comment.Dislikes += 1;

            UpdateCommonFields(@event, eventMetaData);
        }

        private void Handle(CommentLikedEvent commentLikedEvent, string eventDataJson, EventModel @event, EventMetaData eventMetaData)
        {
            _logger.LogInformation($"Handling {nameof(CommentLikedEvent)} .... {Environment.NewLine} {eventDataJson}");

            _operation = DatabaseOperation.Update;
            _currentValueRecord = GetRecord(commentLikedEvent.ParentId);

            ThrowIfRecordIsNull(commentLikedEvent, _currentValueRecord);

            // there must exist a comment record already before we can update it!
            var comment = _currentValueRecord.Comments.First(x => x.PublicId == commentLikedEvent.Id);

            comment.Likes += 1;

            UpdateCommonFields(@event, eventMetaData);
        }

        private void Handle(CommentTextChangedEvent commentTextChangedEvent, string eventDataJson, EventModel @event, EventMetaData eventMetaData)
        {
            _logger.LogInformation($"Handling {nameof(CommentTextChangedEvent)} .... {Environment.NewLine} {eventDataJson}");

            _operation = DatabaseOperation.Update;
            _currentValueRecord = GetRecord(commentTextChangedEvent.ParentId);

            ThrowIfRecordIsNull(commentTextChangedEvent, _currentValueRecord);

            // there must exist a comment record already before we can update it!
            var comment = _currentValueRecord.Comments.First(x => x.PublicId == commentTextChangedEvent.Id);

            comment.CommentText = commentTextChangedEvent.CommentText;

            UpdateCommonFields(@event, eventMetaData);
        }

        private void Handle(CommentAddedEvent commentAddedEvent, string eventDataJson, EventModel @event, EventMetaData eventMetaData)
        {
            _logger.LogInformation($"Handling {nameof(CommentAddedEvent)} .... {Environment.NewLine} {eventDataJson}");

            _operation = DatabaseOperation.Update;
            _currentValueRecord = GetRecord(commentAddedEvent.ParentId);

            ThrowIfRecordIsNull(commentAddedEvent, _currentValueRecord);

            if (_currentValueRecord.Comments == null)
                _currentValueRecord.Comments = new List<ValueCommentRecord>();

            _currentValueRecord.Comments.Add(new ValueCommentRecord
            {
                PublicId = commentAddedEvent.Id,
                ParentId = commentAddedEvent.ParentId,
                CommentText = commentAddedEvent.CommentText,
                UserName = commentAddedEvent.UserName,
                Likes = commentAddedEvent.Likes,
                Dislikes = commentAddedEvent.Dislikes,
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now,
                Id = ObjectId.GenerateNewId()
            });

            UpdateCommonFields(@event, eventMetaData);
        }

        protected override void Connected(EventStoreCatchUpSubscription eventStoreCatchUpSubscription)
        {
            base.Connected(eventStoreCatchUpSubscription);
            _logger.LogInformation("Listening to events from event store...");
        }

        protected override void Dropped(EventStoreCatchUpSubscription sub, SubscriptionDropReason reason,
            Exception exception)
        {
            base.Dropped(sub, reason, exception);
            _logger.LogError(exception, $"Error: Connection to the server is dropped. Drop reason: {reason}");
        }

        protected override void OnException(Exception exception)
        {
            base.OnException(exception);
            _logger.LogError(exception, "An error occurred while processing the event. Please see the exception details");
        }

        private void Handle(ValueChangedEvent valueChangedEvent, string rawJson, EventModel @event, EventMetaData metaData)
        {
            _logger.LogInformation($"Handling {nameof(ValueChangedEvent)} .... {Environment.NewLine} {rawJson}");

            _operation = DatabaseOperation.Update;
            _currentValueRecord = GetRecord(valueChangedEvent.Id);

            ThrowIfRecordIsNull(valueChangedEvent, _currentValueRecord);

            _currentValueRecord.Value = valueChangedEvent.Value;

            UpdateCommonFields(@event, metaData);
        }

        private void Handle(CodeChangedEvent codeChangedEvent, string rawJson, EventModel @event, EventMetaData metaData)
        {
            _logger.LogInformation($"Handling {nameof(CodeChangedEvent)} .... {Environment.NewLine} {rawJson}");
            _operation = DatabaseOperation.Update;
            _currentValueRecord = GetRecord(codeChangedEvent.Id);


            ThrowIfRecordIsNull(codeChangedEvent, _currentValueRecord);

            _currentValueRecord.Code = codeChangedEvent.Code;

            UpdateCommonFields(@event, metaData);
        }

        private void Handle(NameChangedEvent nameChangedEvent, string rawJson, EventModel @event, EventMetaData metaData)
        {
            _logger.LogInformation($"Handling {nameof(NameChangedEvent)} .... {Environment.NewLine} {rawJson}");
            _operation = DatabaseOperation.Update;
            _currentValueRecord = GetRecord(nameChangedEvent.Id);

            ThrowIfRecordIsNull(nameChangedEvent, _currentValueRecord);

            _currentValueRecord.Name = nameChangedEvent.Name;

            UpdateCommonFields(@event, metaData);
        }

        private void Handle(ValueCreatedEvent valueCreatedEvent, string rawJson, EventModel @event, EventMetaData metaData)
        {
            _logger.LogInformation($"Handling {nameof(ValueCreatedEvent)} .... {Environment.NewLine} {rawJson}");
            _operation = DatabaseOperation.Create;

            _currentValueRecord = new ValueRecord
            {
                PublicId = valueCreatedEvent.Id,
                Code = valueCreatedEvent.Code,
                Name = valueCreatedEvent.Name,
                TenantId = valueCreatedEvent.TenantId,
                Value = valueCreatedEvent.Value,
                Version = @event.Version,
                CommitId = metaData.CommitIdHeader,
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now,
                Comments = new List<ValueCommentRecord>()
            };
        }

        private ValueRecord GetRecord(Guid id)
        {
            return _valueRecordsRepository.GetAll()
                .FirstOrDefault(x => x.PublicId.Value == id);
        }

        private void UpdateReadPointer(long position, IClientSessionHandle session)
        {
            _readPointer.Position = position;
            _readPointer.LastModifiedOn = DateTime.Now;
            Task.Run(() =>
            {
                try
                {
                    _readPointerRepository.UpdateAsync(_readPointer, session);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message, exception);
                } }).Wait();
            _logger.LogInformation($"Read pointer updated ...{JsonConvert.SerializeObject(_readPointer)}");
        }

        private void UpdateCommonFields(EventModel @event, EventMetaData metaData)
        {
            _currentValueRecord.Version = @event.Version;
            _currentValueRecord.LastModifiedOn = DateTime.Now;
            _currentValueRecord.CommitId = metaData.CommitIdHeader;
        }

        private void DeleteRecord(EventModel @event, IClientSessionHandle session)
        {
            throw new NotImplementedException();
        }

        private void UpdateRecord(EventModel @event, IClientSessionHandle session)
        {

            Task.Run(() => _valueRecordsRepository.UpdateAsync(_currentValueRecord, session)).Wait();
        }

        private void InsertRecord(EventModel @event, IClientSessionHandle session)
        {
            Task.Run(() => _valueRecordsRepository.AddAsync(_currentValueRecord, session)).Wait();
        }

        private void ThrowIfThereIsHole(IEventModel @event, Guid aggregateId)
        {
            var record = GetRecord(aggregateId);
            if (record == null) return;//record is not created yet!

            if (@event.Version - 1 - record.Version != 0)
                throw new Exception("Error: An event with un-expected version is arrived. " +
                                    $"Expected version: {record.Version + 1}, Event version: {@event.Version}");
        }

        private void ThrowIfRecordIsNull(IEventBase @event, IEntityBase record)
        {
            if (record != null) return;

            throw new Exception($"Cannot process event: '{@event.EventType}'. Error: Record with ID: '{@event.Id}' cannot be found.");
        }
    }

    public enum DatabaseOperation
    {
        Create,
        Update,
        Delete,
        Undefined
    }
}
