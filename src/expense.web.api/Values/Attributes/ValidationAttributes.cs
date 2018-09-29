using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using expense.web.api.Values.Dtos;

namespace expense.web.api.Values.Attributes
{
    public class ValueMaxLengthAttribute : ValidationAttribute
    {
        public int Length { get; }

        public ValueMaxLengthAttribute(int length)
        {
            this.Length = length;
        }

        public override bool IsValid(object value)
        {
            if (Length <= 0) return false;

            // cast fails, but as we are checking max-length,
            // it is safe to return true
            if (!(value is IDtoProp dtoProp)) return true;

            if (string.IsNullOrWhiteSpace(dtoProp.ToString())) return true;

            return dtoProp.ToString().Length <= Length;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessage, name, Length);
        }
    }

    public class ValueMinLengthAttribute : ValueMaxLengthAttribute
    {
        public ValueMinLengthAttribute(int length) : base(length)
        {

        }
        public override bool IsValid(object value)
        {
            if (Length <= 0) return false;

            // casting fails, it means no value provided, we return false,
            // in this case its min-length, and the user must provide the 
            // required number of characters
            if (!(value is IDtoProp dtoProp)) return false;

            if (string.IsNullOrWhiteSpace(dtoProp.ToString())) return false;

            return dtoProp.ToString().Length >= Length;
        }
    }

    public class ValueRequiredAttribute : ValidationAttribute
    {
        private readonly RequiredAttribute _requiredAttribute;

        public ValueRequiredAttribute(bool allowEmptyStrings = false, [CallerMemberName] string propName = null)
        {
            _requiredAttribute = new RequiredAttribute() { AllowEmptyStrings = allowEmptyStrings };
            ErrorMessage = $"{propName} is required!";
        }

        public override bool IsValid(object value)
        {
            return _requiredAttribute.IsValid(!(value is IDtoProp dtoProp) ? string.Empty : dtoProp.ToString());
        }
    }
}
