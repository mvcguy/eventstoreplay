using System;
using System.Threading;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.api.Values.Commands;
using expense.web.api.Values.Commands.Value;
using MediatR;
using Microsoft.Extensions.Logging;

namespace expense.web.api.Values.CommandHandlers
{
    public class UpdateValueCommandHandler : BaseCommandHandler<ValuesRootAggregate, UpdateValueCommandHandler,
        UpdateValueCommand, ValueCommandResponse>
    {
        public UpdateValueCommandHandler(IRepository<ValuesRootAggregate> repository,
            ILogger<UpdateValueCommandHandler> logger) : base(repository, logger)
        {
        }

        public override async Task<ValueCommandResponse> Handle(UpdateValueCommand command,
            CancellationToken cancellationToken)
        {
            var result = new ValueCommandResponse();

            cancellationToken.ThrowIfCancellationRequested();

            var task = Task.Run(() =>
            {
                try
                {
                    var aggregate = Repository.GetById(command.Id);

                    if (aggregate == null)
                    {
                        result.Success = false;
                        result.Message = "Error: Aggregate not found";
                        return;
                    }

                    if (aggregate.Version != command.Version)
                    {
                        result.Success = false;
                        result.Message =
                            "Aggregate version mismatch. Please check that you are passing the correct aggregate version.";
                        return;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    if (command.UpdateCodeCmd != null)
                    {
                        aggregate.ChangeCode(command.UpdateCodeCmd.Code);
                    }

                    if (command.UpdateNameCmd != null)
                    {
                        aggregate.ChangeName(command.UpdateNameCmd.Name);
                    }

                    if (command.UpdateValueCmd != null)
                    {
                        aggregate.ChangeValue(command.UpdateValueCmd.Value);
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    aggregate.Save();

                    result.Success = true;
                    result.Model = aggregate;

                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message, e);
                    result.Success = false;
                    result.Message = $"{e.Message} {Environment.NewLine} {e.StackTrace}";
                }
            }, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            await task;

            return result;
        }

    }
}