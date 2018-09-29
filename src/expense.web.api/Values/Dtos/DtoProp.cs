namespace expense.web.api.Values.Dtos
{
    public interface IDtoProp
    {
        string ToString();

    }

    public class DtoProp<T> : IDtoProp
    {
        public T Value { get; set; }

        public DtoProp()
        {

        }

        public DtoProp(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}