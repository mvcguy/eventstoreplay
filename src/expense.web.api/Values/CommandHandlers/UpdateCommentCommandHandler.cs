using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.api.Values.Commands.Comments;
using Microsoft.Extensions.Logging;

namespace expense.web.api.Values.CommandHandlers
{
    public class UpdateCommentCommandHandler: BaseCommandHandler<ValuesRootAggregate, UpdateCommentCommandHandler,
        UpdateCommentCommand, CommentCommandResponse>
    {
        public UpdateCommentCommandHandler(IRepository<ValuesRootAggregate> repository, ILogger<UpdateCommentCommandHandler> logger) 
            : base(repository, logger)
        {
        }

        public override async Task<CommentCommandResponse> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
        {
            var result = new CommentCommandResponse();

            var task = Task.Run(() =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var aggregate = Repository.GetById(command.ParentId);
                    if (aggregate == null)
                    {
                        result.Success = false;
                        result.Message = "Error: Aggregate not found";
                        return;
                    }

                    if (aggregate.Version != command.ParentVersion)
                    {
                        result.Success = false;
                        result.Message =
                            "Aggregate version mismatch. Please check that you are passing the correct aggregate version.";
                        return;
                    }

                    // a comment must exist before it can be updated!!!
                    var comment = aggregate.Comments.First(x => x.Id == command.Id && x.ParentId == command.ParentId);

                    comment.ChangeCommentText(command.UpdateCommentTextCmd.CommentText);

                    aggregate.Save();
                    result.Success = true;
                    result.Model = comment;

                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.Message = e.Message;
                }

            }, cancellationToken);

            await task;

            return result;
        }
    }
}
