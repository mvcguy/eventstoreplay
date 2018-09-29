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
        private readonly IValuesRepository _repository;
        private readonly ILogger<CreateValueCommandHandler> _logger;

        public CreateValueCommandHandler(IValuesRepository repository, ILogger<CreateValueCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ValueCommandResponse> Handle(CreateValueCommand command, CancellationToken cancellationToken)
        {
            var result = new ValueCommandResponse();

            var task = Task.Run(() =>
            {
                try
                {
                    var valuesAggregate = new ValuesAggregate(_repository);
                    cancellationToken.ThrowIfCancellationRequested();
                    valuesAggregate.CreateValue(new ValueDataModel
                    {
                        TenantId = command.Request.TenantId.GetValueOrDefault(),
                        Name = command.Request.Name,
                        Code = command.Request.Code,
                        Value = command.Request.Value,
                    });

                    cancellationToken.ThrowIfCancellationRequested();
                    valuesAggregate.Save();

                    result.Success = true;
                    result.ValueAggregateModel = valuesAggregate;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    result.Success = false;
                    result.Message = e.Message;
                }
            }, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            await task;

            return result;

        }
    }
}
