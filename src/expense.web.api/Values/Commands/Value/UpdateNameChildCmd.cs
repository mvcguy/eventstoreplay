namespace expense.web.api.Values.Commands.Value
{
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
}