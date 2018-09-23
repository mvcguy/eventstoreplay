using System;
using MediatR;

namespace expense.web.api.Values.Commands
{
    public class CreateValueCommand : IRequest<ValueCommandResponse>
    {
        public ValueCommandRequest Request { get; set; }
    }

    public class UpdateValueCommand : IRequest<ValueCommandResponse>
    {

        public Guid Id { get; set; }

        public long Version { get; set; }

        public UpdateNameChildCmd UpdateNameCmd { get; set; }

        public UpdateCodeChildCmd UpdateCodeCmd { get; set; }

        public UpdateValueChildCmd UpdateValueCmd { get; set; }

        
    }

    public class UpdateNameChildCmd
    {

        public UpdateNameChildCmd()
        {
            
        }

        public UpdateNameChildCmd(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class UpdateCodeChildCmd
    {

        public UpdateCodeChildCmd()
        {
            
        }

        public UpdateCodeChildCmd(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
    }

    public class UpdateValueChildCmd
    {

        public UpdateValueChildCmd()
        {
            
        }

        public UpdateValueChildCmd(string value)
        {
            Value = value;
        }
        public string Value { get; set; }
    }

}
