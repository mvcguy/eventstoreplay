using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;

namespace expense.web.eventstore.EventStoreDataContext
{
    public class StoreContext<TAggregate, TEventModel> where TAggregate : class, IEsAggregate<TEventModel>, new() where TEventModel : IEventModel, new()
    {
        public IEventStoreConnection EventStoreConnection { get; }

        public int ReadPageSize { get; set; }

        public Func<Type, Guid, string> AggregateIdToStreamName { get; set; }

        public StoreContext(IEventStoreConnection eventStoreConnection)
        {
            EventStoreConnection = eventStoreConnection;
            this.ReadPageSize = 10;
            this.AggregateIdToStreamName = (type, guid) => $"{type.Name}-{guid}";
        }

        public TAggregate GetById(Guid id)
        {
            return GetById(id, int.MaxValue);
        }

        public TAggregate GetById(Guid id, long version)
        {
            if (version < 0)
                return default(TAggregate);

            if (id == Guid.Empty)
                return default(TAggregate);

            var streamName = AggregateIdToStreamName(typeof(TAggregate), id);
            var aggregate = new TAggregate();
            aggregate.Id = id;
            aggregate.Version = version;

            long sliceStart = 0;
            aggregate.Events = new List<TEventModel>();
            StreamEventsSlice currentSlice;

            do
            {
                var sliceCount = sliceStart + ReadPageSize <= version
                    ? ReadPageSize
                    : version - sliceStart + 1;

                currentSlice = EventStoreConnection.ReadStreamEventsForwardAsync(streamName, sliceStart, (int)sliceCount, false).Result;

                if (currentSlice.Status == SliceReadStatus.StreamNotFound
                    || currentSlice.Status == SliceReadStatus.StreamDeleted)
                    return default(TAggregate);

                sliceStart = currentSlice.NextEventNumber;

                foreach (var @event in currentSlice.Events)
                {
                    aggregate.Events.Add(new TEventModel
                    {
                        Data = @event.OriginalEvent.Data,
                        Metadata = @event.OriginalEvent.Metadata,
                        SequenceNumber = @event.OriginalEventNumber,
                        Version = @event.OriginalEvent.EventNumber + 1,
                        EventType = @event.OriginalEvent.EventType,
                        IsJson = @event.OriginalEvent.IsJson,
                    });
                }

            } while (version >= currentSlice.NextEventNumber && !currentSlice.IsEndOfStream);
            
            if (!aggregate.Events.Any())
                return default(TAggregate);

            return aggregate;
        }

        public bool Save(TAggregate aggregate)
        {
            var newEvents = aggregate.Events;
            if (newEvents == null || !newEvents.Any()) return false;

            var streamName = AggregateIdToStreamName(aggregate.GetType(), aggregate.Id);

            var originalVersion = aggregate.Version - newEvents.Count - 1;
            var expectedVersion = originalVersion < 0 ? ExpectedVersion.NoStream : originalVersion;
            var eventsToSave = newEvents.Select(e => ToEventData(Guid.NewGuid(), e)).ToList();

            using (var aggregateTransaction = EventStoreConnection.StartTransactionAsync(streamName, expectedVersion).Result)
            {
                try
                {
                    aggregateTransaction.WriteAsync(eventsToSave).Wait();
                    aggregateTransaction.CommitAsync().Wait();
                    aggregate.Events.Clear();
                }
                catch
                {
                    aggregateTransaction.Rollback();
                    throw;
                }
            }
            return true;
        }

        private static EventData ToEventData(Guid newGuid, TEventModel eventModel)
        {
            return new EventData(newGuid, eventModel.EventType, eventModel.IsJson, eventModel.Data, eventModel.Metadata);
        }
    }
}