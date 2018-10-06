using System.Threading;
using System.Threading.Tasks;
using expense.web.api.Values.Aggregate;
using expense.web.api.Values.Aggregate.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace expense.web.api.Values.CommandHandlers
{
    public abstract class BaseCommandHandler<TAggregate, THandler, TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TAggregate : class, IAggregate where TCommand : IRequest<TResponse>
    {
        protected readonly IRepository<TAggregate> Repository;
        protected readonly ILogger<THandler> Logger;

        protected BaseCommandHandler(IRepository<TAggregate> repository, ILogger<THandler> logger)
        {
            Repository = repository;
            Logger = logger;
        }

        public abstract Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
    }
}