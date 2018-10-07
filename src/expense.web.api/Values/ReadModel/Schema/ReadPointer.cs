namespace expense.web.api.Values.ReadModel.Schema
{
    public class ReadPointer : EntityBase
    {
        public string SourceName { get; set; }

        public long? Position { get; set; }

    }
}