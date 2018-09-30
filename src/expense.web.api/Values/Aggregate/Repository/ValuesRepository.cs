﻿using System;
using System.Linq;
using System.Text;
using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events;
using expense.web.api.Values.Aggregate.Model;
using expense.web.eventstore.EventStoreDataContext;
using Newtonsoft.Json;

namespace expense.web.api.Values.Aggregate.Repository
{
    public class ValuesRepository : IValuesRepository
    {
        private readonly StoreContext<ValuesAggregate, EventModel> _context;

        public ValuesRepository(StoreContext<ValuesAggregate, EventModel> context)
        {
            _context = context;
        }

        public ValuesAggregate GetById(Guid id)
        {
            return GetById(id, int.MaxValue);
        }
        
        public ValuesAggregate GetById(Guid id, long version)
        {
            var temp = version;
            if (temp <= 0)
            {
                temp = int.MaxValue;
            }

            var aggregate = _context.GetById(id, temp);

            if (aggregate == null || !aggregate.Events.Any()) return null;

            return CreateAggregate(aggregate);
        }

        public bool Save(ValuesAggregate aggregate)
        {
            return _context.Save(aggregate);
        }

        private ValuesAggregate CreateAggregate(IEsAggregate<EventModel> esAggregate)
        {
            var aggregate = new ValuesAggregate(esAggregate.Id, esAggregate.Events.Max(x => x.Version), this);

            foreach (var aggregateEvent in esAggregate.Events)
            {
                // TODO: do we need Metadata in the public model?
                var metaDataJson = Encoding.UTF8.GetString(aggregateEvent.Metadata);
                var eventDataJson = Encoding.UTF8.GetString(aggregateEvent.Data);
                switch (aggregateEvent.EventType)
                {
                    case ValueAggregateConstants.EventTypes.ValueCreated:
                        aggregate.Handle(JsonConvert.DeserializeObject<ValueCreatedEvent>(eventDataJson));
                        break;
                    case ValueAggregateConstants.EventTypes.NameChanged:
                        aggregate.Handle(JsonConvert.DeserializeObject<NameChangedEvent>(eventDataJson));
                        break;
                    case ValueAggregateConstants.EventTypes.CodeChanged:
                        aggregate.Handle(JsonConvert.DeserializeObject<CodeChangedEvent>(eventDataJson));
                        break;
                    case ValueAggregateConstants.EventTypes.ValueChanged:
                        aggregate.Handle(JsonConvert.DeserializeObject<ValueChangedEvent>(eventDataJson));
                        break;
                    default:
                        break;
                }
            }
            return aggregate;
        }

        public bool Exists(IValueAggregateModel model)
        {
            throw new NotImplementedException();
        }

        public bool Exists(IValueAggregateModel model, out Guid aggregateId)
        {
            throw new NotImplementedException();
        }
    }
}