using Mc2.CrudTest.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Mc2.CrudTest.Domain.ValueObjects
{
    public class BankAccountNumber
    {
        public string Value { get; private set; }

        private BankAccountNumber(string value)
        {
            Value = value;
        }

        public static BankAccountNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidBankAccountNumberException(value);

            if (!Regex.IsMatch(value, @"^\d{16}$"))
                throw new InvalidBankAccountNumberException(value);

            return new BankAccountNumber(value);
        }

        public override string ToString() => Value;
    }
}
