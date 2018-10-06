namespace expense.web.api.Values.Commands.Value
{
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