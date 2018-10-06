using System;
using expense.web.api.Values.Attributes;

namespace expense.web.api.Values.Dtos
{
    public class ValueViewModel
    {
        public Guid? Id { get; set; }

        [ValueRequired()]
        public DtoProp<int?> TenantId { get; set; }

        [ValueMaxLength(6, ErrorMessage = "The length of {0} cannot be greater than {1} characters")]
        [ValueMinLength(2, ErrorMessage = "The length of {0} cannot be less than {1} characters")]
        public DtoProp<string> Code { get; set; }

        [ValueMaxLength(150, ErrorMessage = "The length of {0} cannot be greater than {1} characters")]
        [ValueMinLength(2, ErrorMessage = "The length of {0} cannot be less than {1} characters")]
        public DtoProp<string> Name { get; set; }

        [ValueMaxLength(250, ErrorMessage = "The length of {0} cannot be greater than {1} characters")]
        [ValueMinLength(2, ErrorMessage = "The length of {0} cannot be less than {1} characters")]
        public DtoProp<string> Value { get; set; }

        public DtoProp<long?> Version { get; set; }
    }
}
