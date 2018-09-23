using System;
using System.Collections.Generic;
using System.Threading;
using expense.web.api.Values.Aggregate.Constants;
using expense.web.api.Values.Aggregate.Model;

namespace expense.web.api.Values.Aggregate.Events
{
    public class EventBase : IEventBase
    {
        public EventBase()
        {
            _metaData = new Dictionary<string, object>();
        }

        public EventBase(IValueAggregateModel model, string eventType, string eventClrType)
        {
            TenantId = model.TenantId;
            Id = model.Id;
            CreatedDateTimeUtc = DateTime.UtcNow;
            EventType = eventType;
            ServerCulture = Thread.CurrentThread.CurrentCulture.Name;
            _metaData = new Dictionary<string, object>()
            {
                {ValueAggregateConstants.EventMetaDataHeaders.AggregateClrTypeHeader,model.GetType().AssemblyQualifiedName },
                {ValueAggregateConstants.EventMetaDataHeaders.EventClrTypeHeader, eventClrType },
                {ValueAggregateConstants.EventMetaDataHeaders.CommitIdHeader, model.CommitId }
            };
        }

        public int TenantId { get; set; }

        public Guid Id { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }

        public string ServerCulture { get; set; }

        public string EventType { get; set; }
        
        private readonly IDictionary<string, object> _metaData;

        public IDictionary<string, object> GetMetaData()
        {
            return _metaData;
        }
    }
}