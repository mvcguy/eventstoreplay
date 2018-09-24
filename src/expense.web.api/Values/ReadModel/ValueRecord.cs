using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace expense.web.api.Values.ReadModel
{
    public class ValueRecord : IEntityBase
    {
        public ObjectId Id { get; set; }

        public int? TenantId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public Guid PublicId { get; set; }

        public long? Version { get; set; }

        public Guid CommitId { get; set; }
    }
}
