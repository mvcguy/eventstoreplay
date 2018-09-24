using System;
using System.Threading.Tasks;
using expense.web.eventstore.EventStoreDataContext;

namespace expense.web.eventstore.EventStoreSubscriber
{
    public interface IEventStoreSubscriber
    {
        bool IsStarted { get; }

        void OnEvent(EventModel @event);

        Task Start(long? checkpoint);
    }
}