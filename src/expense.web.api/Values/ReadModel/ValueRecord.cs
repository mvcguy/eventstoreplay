using System;

namespace expense.web.api.Values.ReadModel
{
    public class ValueRecord : EntityBase
    {
        public int? TenantId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
        
        public long? Version { get; set; }

        public Guid CommitId { get; set; }
    }

    public class ReadPointer : EntityBase
    {
        public string SourceName { get; set; }

        public long? Position { get; set; }

    }
}
