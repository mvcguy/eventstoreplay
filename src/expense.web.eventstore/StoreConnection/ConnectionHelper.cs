using System;
using System.Threading;
using EventStore.ClientAPI;

namespace expense.web.eventstore.StoreConnection
{
    public static class ConnectionHelper
    {
        private static int _nextConnId = -1;

        public static IEventStoreConnection Create(string connection, ILogger logger, TcpType tcpType = TcpType.Normal)
        {
            
            return EventStoreConnection.Create(Settings(tcpType, logger), new Uri(connection), string.Format("ESC-{0}", Interlocked.Increment(ref _nextConnId)));
        }
        
        private static ConnectionSettingsBuilder Settings(TcpType tcpType, ILogger customerLogger)
        {

            var settings = ConnectionSettings.Create()
                //.UseCustomLogger(customerLogger)
                .EnableVerboseLogging()
                .LimitReconnectionsTo(10)
                .LimitRetriesForOperationTo(100)
                .SetTimeoutCheckPeriodTo(TimeSpan.FromMilliseconds(100))
                .SetReconnectionDelayTo(TimeSpan.Zero)
                .FailOnNoServerResponse()
                .SetOperationTimeoutTo(TimeSpan.FromDays(1));
            if (tcpType == TcpType.Ssl)
                settings.UseSslConnection("ES", false);
            return settings;
        }
    }
}