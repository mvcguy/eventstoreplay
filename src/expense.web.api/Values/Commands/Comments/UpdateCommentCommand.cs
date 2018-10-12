using System;
using MediatR;

namespace expense.web.api.Values.Commands.Comments
{
    public class UpdateCommentCommand : IRequest<CommentCommandResponse>
    {
        public long ParentVersion { get; set; }

        public Guid ParentId { get; set; }

        public Guid Id { get; set; }

        public UpdateCommentTextChildCmd UpdateCommentTextCmd { get; set; }

        public CommentLikedChildCmd CommentLikedChildCmd { get; set; }

        public CommentDislikedChildCmd CommentDislikedChildCmd { get; set; }
    }
}