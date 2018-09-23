namespace expense.web.api.Values.Commands
{
    public class ValueCommandRequest
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public int? TenantId { get; set; }

        public string Code { get; set; }
    }
}