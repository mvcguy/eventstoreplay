using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace expense.web.api.Values.Commands.Comments
{
    public class AddCommentCommand : IRequest<AddCommentCommandResponse>
    {
        public long ParentVersion { get; set; }

        public Guid ParentId { get; set; }

        public Guid Id { get; set; }

        public int? TenantId { get; set; }

        public string UserName { get; set; }

        public string CommentText { get; set; }
    }
}
