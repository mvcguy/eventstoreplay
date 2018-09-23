using System;

namespace expense.web.api.Values.Dtos
{
    public class ValueViewModel
    {
        public Guid Id { get; set; }

        public DtoProp<int> TenantId { get; set; }

        public DtoProp<string> Code { get; set; }

        public DtoProp<string> Name { get; set; }

        public DtoProp<string> Value { get; set; }

        public DtoProp<long> Version { get; set; }
    }

    public class DtoProp<T>
    {
        public T Value { get; set; }

        public DtoProp()
        {
            
        }

        public DtoProp(T value)
        {
            Value = value;
        }
    }
}
