using System;
using System.Collections.Generic;

namespace expense.web.api.Values.ReadModel.Schema
{
    public class ValueRecord : EntityBase
    {
        public int? TenantId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
        
        public long? Version { get; set; }

        public Guid CommitId { get; set; }

        public IList<ValueCommentRecord> Comments { get; set; }
    }

    public class ValueCommentRecord : EntityBase
    {
        // Id of the aggregate root
        public Guid? ParentId { get; set; }

        public string CommentText { get; set; }

        public string UserName { get; set; }

        public int? Likes { get; set; }

        public int? Dislikes { get; set; }
    }
}
