using System;
using System.Threading;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate;
using expense.web.api.Values.Aggregate.Model;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.api.Values.Commands.Comments;
using Microsoft.Extensions.Logging;

namespace expense.web.api.Values.CommandHandlers
{
    public class AddCommentCommandHandler
        : BaseCommandHandler<ValuesRootAggregate,
            AddCommentCommandHandler, AddCommentCommand, AddCommentCommandResponse>
    {
        public AddCommentCommandHandler(IRepository<ValuesRootAggregate> repository, ILogger<AddCommentCommandHandler> logger) : base(repository, logger)
        {
        }

        public override async Task<AddCommentCommandResponse> Handle(AddCommentCommand command, CancellationToken cancellationToken)
        {
            var result = new AddCommentCommandResponse();

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

                    var comment = aggregate.AddComment(new ValueCommentAggregateChildDataModel
                    {
                        CommentText = command.CommentText,
                        UserName = command.UserName
                    });

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