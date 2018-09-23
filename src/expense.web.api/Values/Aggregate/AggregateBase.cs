using System;
using System.Collections.Generic;
using System.Text;
using expense.web.api.Values.Aggregate.Events;
using expense.web.eventstore.EventStoreDataContext;
using Newtonsoft.Json;

namespace expense.web.api.Values.Aggregate
{
    public abstract class AggregateBase<TEventModel> : IAggregate, IEsAggregate<TEventModel> where TEventModel: IEventModel, new()
    {
        protected AggregateBase()
        {
            Events = new List<TEventModel>();
        }

        public IList<TEventModel> Events { get; set; }
        
        public Guid Id { get; set; }

        public long Version { get; set; }

        public void ApplyEvent(object @event)
        {
            AppendEvent(@event);
            IncrementVersion();
        }

        public virtual void IncrementVersion()
        {
            Version++;
        }

        public virtual void AppendEvent(object @event)
        {
            // TODO: Should we throw an exception if the event is not derived from IEventBase?
            if (!(@event is IEventBase baseEvent)) return;

            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, Formatting.None));
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(baseEvent.GetMetaData(), Formatting.None));
            var typeName = @event.GetType().Name;

            Events.Add(new TEventModel()
            {
                IsJson = true,
                EventType = typeName,
                Data = data,
                Metadata = metadata
            });
        }

        public bool Equals(IAggregate other)
        {
            return Id.Equals(other?.Id) && Version == other?.Version;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IAggregate)obj);
        }
        
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}