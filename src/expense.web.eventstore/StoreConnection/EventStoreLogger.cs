﻿using System;
using Microsoft.Extensions.Logging;
using IEventStoreLogger = EventStore.ClientAPI.ILogger;


namespace expense.web.eventstore.StoreConnection
{
    public class EventStoreLogger : IEventStoreLogger
    {
        private readonly ILogger<EventStoreLogger> _logger;

        public EventStoreLogger(ILogger<EventStoreLogger> logger)
        {
            _logger = logger;
        }

        public void Error(string format, params object[] args)
        {
            _logger.LogError(format, args);
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            _logger.LogError(ex, format, args);
        }

        public void Info(string format, params object[] args)
        {
            _logger.LogInformation(format, args);
        }

        public void Info(Exception ex, string format, params object[] args)
        {
            _logger.LogInformation(ex, format, args);
        }

        public void Debug(string format, params object[] args)
        {
            _logger.LogDebug(format, args);
        }

        public void Debug(Exception ex, string format, params object[] args)
        {
            _logger.LogDebug(ex, format, args);
        }
    }
}