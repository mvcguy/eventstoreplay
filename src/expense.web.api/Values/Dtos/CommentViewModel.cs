using System;

namespace expense.web.api.Values.Dtos
{
    public class CommentViewModel
    {
        public Guid? Id { get; set; }

        public Guid? ParentId { get; set; }
        
        public DtoProp<long?> ParentVersion { get; set; }

        public DtoProp<int?> TenantId { get; set; }

        public DtoProp<string> CommentText { get; set; }

        public DtoProp<string> UserName { get; set; }

        public DtoProp<int?> Likes { get; set; }

        public DtoProp<int?> Dislikes { get; set; }
    }
}