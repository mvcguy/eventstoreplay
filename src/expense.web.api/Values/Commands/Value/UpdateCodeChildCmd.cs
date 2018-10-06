namespace expense.web.api.Values.Commands.Value
{
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
}