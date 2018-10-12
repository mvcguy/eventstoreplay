using System;
using expense.web.api.Values.Attributes;

namespace expense.web.api.Values.Dtos
{
    public class CommentViewModel
    {
        public Guid? Id { get; set; }

        public Guid? ParentId { get; set; }

        [ValueRequired()]
        public DtoProp<long?> ParentVersion { get; set; }

        [ValueRequired()]
        public DtoProp<int?> TenantId { get; set; }

        [ValueMaxLength(2500, ErrorMessage = "The length of {0} cannot be greater than {1} characters")]
        [ValueMinLength(10, ErrorMessage = "The length of {0} cannot be less than {1} characters")]
        public DtoProp<string> CommentText { get; set; }

        [ValueRequired()]
        public DtoProp<string> UserName { get; set; }

        public DtoProp<int?> Likes { get; set; }

        public DtoProp<int?> Dislikes { get; set; }
    }
}