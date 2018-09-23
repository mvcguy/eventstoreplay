namespace expense.web.eventstore.EventStoreDataContext
{
    public interface IEventModel
    {
        byte[] Data { get; set; }
        string EventType { get; set; }
        bool IsJson { get; set; }
        byte[] Metadata { get; set; }
        long SequenceNumber { get; set; }
        long Version { get; set; }
    }
}