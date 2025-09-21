using Mc2.CrudTest.Domain.Exceptions;

namespace Mc2.CrudTest.Domain.ValueObjects
{
    public class PhoneNumber
    {
        public string Value { get; private set; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static PhoneNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidPhoneNumberException(value);

            if (!value.StartsWith("+") || value.Length < 10)
                throw new InvalidPhoneNumberException(value);

            return new PhoneNumber(value);
        }

        public override string ToString() => Value;
    }
}
