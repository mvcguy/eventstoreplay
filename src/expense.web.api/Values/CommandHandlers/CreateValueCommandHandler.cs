using System;
using System.Threading;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate;
using expense.web.api.Values.Aggregate.Model;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.api.Values.Commands;
using expense.web.api.Values.Commands.Value;
using Microsoft.Extensions.Logging;

namespace expense.web.api.Values.CommandHandlers
{
    public class CreateValueCommandHandler :
        BaseCommandHandler<ValuesRootAggregate, CreateValueCommandHandler, CreateValueCommand, ValueCommandResponse>
    {
        public CreateValueCommandHandler(IRepository<ValuesRootAggregate> repository,
            ILogger<CreateValueCommandHandler> logger) : base(repository, logger) { }

        public override async Task<ValueCommandResponse> Handle(CreateValueCommand command, CancellationToken cancellationToken)
        {
            var result = new ValueCommandResponse();

            var task = Task.Run(() =>
            {
                try
                {
                    var valuesAggregate = new ValuesRootAggregate(Repository);
                    cancellationToken.ThrowIfCancellationRequested();
                    valuesAggregate.CreateValue(new ValuesRootAggregateDataModel
                    {
                        TenantId = command.TenantId.GetValueOrDefault(),
                        Name = command.Name,
                        Code = command.Code,
                        Value = command.Value,
                    });

                    cancellationToken.ThrowIfCancellationRequested();
                    valuesAggregate.Save();

                    result.Success = true;
                    result.Model = valuesAggregate;
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
