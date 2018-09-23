using System;
using System.Threading.Tasks;
using expense.web.eventstore.EventStoreDataContext;

namespace expense.web.eventstore.EventSubscriber
{
    public interface IEventStoreSubscriber
    {
        Action<object> ConnectedDef { get; set; }

        Action<object> DroppedDef { get; set; }

        Action<object> HandlerDef { get; set; }

        bool IsStarted { get; }

        void OnEvent(EventModel @event);

        Task Start(long? checkpoint);
    }
}