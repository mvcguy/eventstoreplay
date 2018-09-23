﻿using System;
using System.Text;
using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events;
using expense.web.eventstore.EventStoreDataContext;
using expense.web.eventstore.EventSubscriber;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace expense.web.api.Values.ReadModel
{
    public class ValuesEventConsumer : EventStoreSubscriberBase
    {
        private readonly ILogger _logger;

        public ValuesEventConsumer(IOptions<SubscriberOptions> options, ILogger logger, IEventStoreConnection eventStoreConnection) : base(options, logger, eventStoreConnection)
        {
            _logger = logger;
        }

        public override void OnEvent(EventModel @event)
        {
            var metaDataJson = Encoding.UTF8.GetString(@event.Metadata);
            var eventDataJson = Encoding.UTF8.GetString(@event.Data);
            switch (@event.EventType)
            {
                case ValueAggregateConstants.EventTypes.ValueCreated:
                    Handle(JsonConvert.DeserializeObject<ValueCreatedEvent>(eventDataJson), eventDataJson);
                    break;
                case ValueAggregateConstants.EventTypes.NameChanged:
                    Handle(JsonConvert.DeserializeObject<NameChangedEvent>(eventDataJson), eventDataJson);
                    break;
                case ValueAggregateConstants.EventTypes.CodeChanged:
                    Handle(JsonConvert.DeserializeObject<CodeChangedEvent>(eventDataJson), eventDataJson);
                    break;
                case ValueAggregateConstants.EventTypes.ValueChanged:
                    Handle(JsonConvert.DeserializeObject<ValueChangedEvent>(eventDataJson), eventDataJson);
                    break;
                default:
                    _logger.LogInformation($"Unknown event type: {@event.EventType}, Payload: {eventDataJson}");
                    break;
            }
        }

        private void Handle(ValueChangedEvent valueChangedEvent, string rawJson)
        {
            _logger.LogInformation($"Handling.... {Environment.NewLine}, {rawJson}");
        }

        private void Handle(CodeChangedEvent codeChangedEvent, string rawJson)
        {
            _logger.LogInformation($"Handling.... {Environment.NewLine}, {rawJson}");
        }

        private void Handle(NameChangedEvent nameChangedEvent, string rawJson)
        {
            _logger.LogInformation($"Handling.... {Environment.NewLine}, {rawJson}");
        }

        private void Handle(ValueCreatedEvent deserializeObject, string rawJson)
        {
            _logger.LogInformation($"Handling.... {Environment.NewLine}, {rawJson}");
        }
    }

    
}
