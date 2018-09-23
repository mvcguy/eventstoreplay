namespace expense.web.eventstore.EventStoreDataContext
{
    public class EventModel : IEventModel
    {
        public byte[] Data { get; set; }

        public byte[] Metadata { get; set; }

        public long SequenceNumber { get; set; }

        public long Version { get; set; }

        public string EventType { get; set; }

        public bool IsJson { get; set; }
    }
}