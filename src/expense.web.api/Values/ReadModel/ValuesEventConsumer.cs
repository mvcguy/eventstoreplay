using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events;
using expense.web.eventstore.EventStoreDataContext;
using expense.web.eventstore.EventStoreSubscriber;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace expense.web.api.Values.ReadModel
{
    public class ValuesEventConsumer : EventStoreSubscriberBase
    {
        private readonly IMongoClient _mongoClient;
        private readonly ILogger<ValuesEventConsumer> _logger;
        private readonly IReadModelRepository<ValueRecord> _valueRecordsRepository;
        private readonly IReadModelRepository<ReadPointer> _readPointerRepistory;
        private ValueRecord _currentValueRecord;
        private DatabaseOperation _operation;
        private ReadPointer _readPointer;

        public ValuesEventConsumer(IOptions<SubscriberOptions> options,
            IMongoClient mongoClient,
            ILogger<ValuesEventConsumer> logger,
            IReadModelRepository<ValueRecord> valueRecordsRepository,
            IReadModelRepository<ReadPointer> readPointerRepistory,
            IEventStoreConnection eventStoreConnection) : base(options, logger, eventStoreConnection)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _valueRecordsRepository = valueRecordsRepository;
            _readPointerRepistory = readPointerRepistory;

            // there must be atleast one recrod!!!
            _readPointer = _readPointerRepistory.GetAll()
                .First(x => x.SourceName==options.Value.TopicName);
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
                            InsertRecord(@event);
                            UpdateReadPointer(@event.SequenceNumber);
                            break;
                        case DatabaseOperation.Update:
                            UpdateRecord(@event);
                            UpdateReadPointer(@event.SequenceNumber);
                            break;
                        case DatabaseOperation.Delete:
                            DeleteRecord(@event);
                            UpdateReadPointer(@event.SequenceNumber);
                            break;
                    }
                    session.CommitTransaction();

                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    session.AbortTransaction();
                }
            }

        }


        private void Handle(ValueChangedEvent valueChangedEvent, string rawJson, EventModel @event, EventMetaData metaData)
        {
            _logger.LogInformation($"Handling {nameof(ValueChangedEvent)} .... {Environment.NewLine} {rawJson}");

            _operation = DatabaseOperation.Update;
            _currentValueRecord = GetRecord(valueChangedEvent.Id);
            _currentValueRecord.Value = valueChangedEvent.Value;

            UpdateCommonFields(@event, metaData);
        }


        private void Handle(CodeChangedEvent codeChangedEvent, string rawJson, EventModel @event, EventMetaData metaData)
        {
            _logger.LogInformation($"Handling {nameof(CodeChangedEvent)} .... {Environment.NewLine} {rawJson}");
            _operation = DatabaseOperation.Update;
            _currentValueRecord = GetRecord(codeChangedEvent.Id);
            _currentValueRecord.Code = codeChangedEvent.Code;

            UpdateCommonFields(@event, metaData);
        }
        
        private void Handle(NameChangedEvent nameChangedEvent, string rawJson, EventModel @event, EventMetaData metaData)
        {
            _logger.LogInformation($"Handling {nameof(NameChangedEvent)} .... {Environment.NewLine} {rawJson}");
            _operation = DatabaseOperation.Update;
            _currentValueRecord = GetRecord(nameChangedEvent.Id);
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

            };
        }

        private ValueRecord GetRecord(Guid id)
        {
            return _valueRecordsRepository.GetAll()
                .First(x => x.PublicId.Value == id);
        }

        private void UpdateReadPointer(long position)
        {
            _readPointer.Position = position;
            _readPointer.LastModifiedOn = DateTime.Now;
            Task.Run(() => { _readPointerRepistory.UpdateAsync(_readPointer); }).Wait();
        }
        
        private void UpdateCommonFields(EventModel @event, EventMetaData metaData)
        {
            _currentValueRecord.Version = @event.Version;
            _currentValueRecord.LastModifiedOn = DateTime.Now;
            _currentValueRecord.CommitId = metaData.CommitIdHeader;
        }
        
        private void DeleteRecord(EventModel @event)
        {
            throw new NotImplementedException();
        }

        private void UpdateRecord(EventModel @event)
        {
            Task.Run(() => _valueRecordsRepository.UpdateAsync(_currentValueRecord)).Wait();
        }

        private void InsertRecord(EventModel @event)
        {
            Task.Run(() => _valueRecordsRepository.AddAsync(_currentValueRecord)).Wait();
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
