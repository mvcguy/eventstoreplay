using System;
using System.Threading;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate;
using expense.web.api.Values.Aggregate.Model;
using expense.web.api.Values.Aggregate.Repository;
using expense.web.api.Values.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace expense.web.api.Values.CommandHandlers
{
    public class CreateValueCommandHandler : IRequestHandler<CreateValueCommand, ValueCommandResponse>
    {
        private readonly IRepository<ValuesRootAggregate> _rootAggregateRepository;
        private readonly ILogger<CreateValueCommandHandler> _logger;

        public CreateValueCommandHandler(IRepository<ValuesRootAggregate> rootAggregateRepository, ILogger<CreateValueCommandHandler> logger)
        {
            _rootAggregateRepository = rootAggregateRepository;
            _logger = logger;
        }

        public async Task<ValueCommandResponse> Handle(CreateValueCommand command, CancellationToken cancellationToken)
        {
            var result = new ValueCommandResponse();

            var task = Task.Run(() =>
            {
                try
                {
                    var valuesAggregate = new ValuesRootAggregate(_rootAggregateRepository);
                    cancellationToken.ThrowIfCancellationRequested();
                    valuesAggregate.CreateValue(new ValuesRootAggregateDataModel
                    {
                        TenantId = command.Request.TenantId.GetValueOrDefault(),
                        Name = command.Request.Name,
                        Code = command.Request.Code,
                        Value = command.Request.Value,
                    });

                    cancellationToken.ThrowIfCancellationRequested();
                    valuesAggregate.Save();

                    result.Success = true;
                    result.ValuesRootAggregateModel = valuesAggregate;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
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
