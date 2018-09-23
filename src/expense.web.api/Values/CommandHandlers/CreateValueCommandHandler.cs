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

    public class UpdateValueCommandHandler : IRequestHandler<UpdateValueCommand, ValueCommandResponse>
    {
        private readonly IValuesRepository _repository;
        private readonly ILogger<CreateValueCommandHandler> _logger;

        public UpdateValueCommandHandler(IValuesRepository repository, ILogger<CreateValueCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        public async Task<ValueCommandResponse> Handle(UpdateValueCommand request, CancellationToken cancellationToken)
        {
            var result = new ValueCommandResponse();

            cancellationToken.ThrowIfCancellationRequested();

            var task = Task.Run(() =>
            {
                try
                {
                    var aggregate = _repository.GetById(request.Id);

                    if (aggregate == null)
                    {
                        result.Success = false;
                        result.Message = "Error: Aggregate not found";
                        return;
                    }

                    if (aggregate.Version != request.Version)
                    {
                        result.Success = false;
                        result.Message = "Aggregate version mismatch. Please check that you are passing the correct aggregate version.";
                        return;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.UpdateCodeCmd != null)
                    {
                        aggregate.ChangeCode(request.UpdateCodeCmd.Code);
                    }

                    if (request.UpdateNameCmd != null)
                    {
                        aggregate.ChangeName(request.UpdateNameCmd.Name);
                    }

                    if (request.UpdateValueCmd != null)
                    {
                        aggregate.ChangeValue(request.UpdateValueCmd.Value);
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    aggregate.Save();

                    result.Success = true;
                    result.ValueAggregateModel = aggregate;

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
