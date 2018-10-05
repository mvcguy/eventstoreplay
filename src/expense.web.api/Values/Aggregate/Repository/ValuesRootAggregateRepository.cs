using System;
using System.Linq;
using System.Text;
using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Events.Root;
using expense.web.eventstore.EventStoreDataContext;
using Newtonsoft.Json;

namespace expense.web.api.Values.Aggregate.Repository
{
    public class ValuesRootAggregateRepository : IRepository<ValuesRootAggregate>
    {
        private readonly StoreContext<ValuesRootAggregate, EventModel> _context;

        public ValuesRootAggregateRepository(StoreContext<ValuesRootAggregate, EventModel> context)
        {
            _context = context;
        }

        public ValuesRootAggregate GetById(Guid id)
        {
            return GetById(id, int.MaxValue);
        }
        
        public ValuesRootAggregate GetById(Guid id, long version)
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

        public bool Save(ValuesRootAggregate rootAggregate)
        {
            return _context.Save(rootAggregate);
        }

        private ValuesRootAggregate CreateAggregate(IEsAggregate<EventModel> esAggregate)
        {
            var aggregate = new ValuesRootAggregate(esAggregate.Id, esAggregate.Events.Max(x => x.Version), this);

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

        public bool Exists(ValuesRootAggregate model)
        {
            throw new NotImplementedException();
        }

        public bool Exists(ValuesRootAggregate model, out Guid aggregateId)
        {
            throw new NotImplementedException();
        }
    }
}