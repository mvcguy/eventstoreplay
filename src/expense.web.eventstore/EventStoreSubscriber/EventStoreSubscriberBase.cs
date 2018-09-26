using System;
using System.Threading;
using System.Threading.Tasks;
using expense.web.eventstore.EventStoreDataContext;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ILogger = Microsoft.Extensions.Logging.ILogger;


namespace expense.web.eventstore.EventStoreSubscriber
{
    public abstract class EventStoreSubscriberBase : IEventStoreSubscriber
    {
        protected readonly IOptions<SubscriberOptions> Options;
        private readonly ILogger _logger;
        private readonly IEventStoreConnection _eventStoreConnection;

        public bool IsStarted { get; private set; }
        
        protected EventStoreSubscriberBase(IOptions<SubscriberOptions> options, 
            ILogger logger, 
            IEventStoreConnection eventStoreConnection)
        {
            Options = options;
            _logger = logger;
            _eventStoreConnection = eventStoreConnection;
        }

        public virtual Task Start(long? checkpoint)
        {
            var task = Task.Run(() =>
            {
                _eventStoreConnection.ConnectAsync().Wait();

                var manualResetEvent = new ManualResetEvent(false);

                try
                {
                    var subscription = _eventStoreConnection.SubscribeToStreamFrom(Options.Value.TopicName,
                        checkpoint,
                        CatchUpSubscriptionSettings.Default,
                        HandleEvent,
                        Connected,
                        Dropped);
                    IsStarted = true;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    manualResetEvent.Set();
                }

                manualResetEvent.WaitOne();
            });

            return task;
        }
        
        protected virtual void Connected(EventStoreCatchUpSubscription eventStoreCatchUpSubscription)
        {
           
        }

        protected virtual void Dropped(EventStoreCatchUpSubscription eventStoreCatchUpSubscription, SubscriptionDropReason subscriptionDropReason, Exception exception)
        {
           
        }

        protected virtual Task HandleEvent(EventStoreCatchUpSubscription eventStoreCatchUpSubscription, ResolvedEvent resolvedEvent)
        {
            try
            {
                OnEvent(new EventModel
                {
                    Data = resolvedEvent.Event.Data,
                    Metadata = resolvedEvent.Event.Metadata,
                    Version = resolvedEvent.Event.EventNumber + 1,
                    SequenceNumber = resolvedEvent.OriginalEventNumber,// checkpoint
                    IsJson = resolvedEvent.Event.IsJson,
                    EventType = resolvedEvent.Event.EventType
                });
            }
            catch (Exception e)
            {
                OnEventException(e, resolvedEvent);
            }

            return Task.CompletedTask;
        }

        public abstract void OnEvent(EventModel @event);

        protected virtual void OnEventException(Exception exception, ResolvedEvent resolvedEvent)
        {
            
        }
    }
}
