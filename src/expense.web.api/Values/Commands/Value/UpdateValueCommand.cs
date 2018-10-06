using System;
using MediatR;

namespace expense.web.api.Values.Commands.Value
{
    public class UpdateValueCommand : IRequest<ValueCommandResponse>
    {
        public Guid Id { get; set; }

        public long Version { get; set; }

        public UpdateNameChildCmd UpdateNameCmd { get; set; }

        public UpdateCodeChildCmd UpdateCodeCmd { get; set; }

        public UpdateValueChildCmd UpdateValueCmd { get; set; }
    }
}