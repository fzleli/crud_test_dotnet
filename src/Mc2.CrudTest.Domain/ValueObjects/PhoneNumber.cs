using Mc2.CrudTest.Domain.Common;
using Mc2.CrudTest.Domain.Exceptions;

namespace Mc2.CrudTest.Domain.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public ulong Value { get; private set; }

        private PhoneNumber(ulong value) => Value = value;

        public static PhoneNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidPhoneNumberException(value);

            if (!value.StartsWith("+") || value.Length < 10)
                throw new InvalidPhoneNumberException(value);

            var digits = value.Substring(1).Replace(" ", "").Replace("-", "");

            if (!digits.All(char.IsDigit))
                throw new InvalidPhoneNumberException(value);

            if (!ulong.TryParse(digits, out var numericValue))
                throw new InvalidPhoneNumberException(value);

            return new PhoneNumber(numericValue);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
