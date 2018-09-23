using System;
using System.Collections.Generic;

namespace expense.web.api.Values.Aggregate.Events
{
    public interface IEventBase
    {
        int TenantId { get; set; }

        Guid Id { get; set; }

        DateTime CreatedDateTimeUtc { get; set; }

        string ServerCulture { get; set; }

        string EventType { get; set; }

        IDictionary<string, object> GetMetaData();
    }
}