using Mc2.CrudTest.Domain.Common;
using Mc2.CrudTest.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Mc2.CrudTest.Domain.ValueObjects
{
    public class Email : ValueObject
    {
        public string Value { get; private set; }

        private Email(string value) => Value = value;

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidEmailException(value);

            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!regex.IsMatch(value))
                throw new InvalidEmailException(value);

            return new Email(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
