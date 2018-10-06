using MediatR;

namespace expense.web.api.Values.Commands.Value
{
    public class CreateValueCommand : IRequest<ValueCommandResponse>
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public int? TenantId { get; set; }

        public string Code { get; set; }
    }
}
