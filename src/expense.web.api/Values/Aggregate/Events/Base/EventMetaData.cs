using System;

namespace expense.web.api.Values.Aggregate.Events.Base
{
    public class EventMetaData
    {
        public string AggregateClrTypeHeader { get; set; }

        public string EventClrTypeHeader { get; set; }

        public Guid CommitIdHeader { get; set; }
    }
}