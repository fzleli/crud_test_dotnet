namespace Mc2.CrudTest.Domain.Exceptions
{
    public class InvalidBankAccountNumberException : Exception
    {
        public InvalidBankAccountNumberException(string accountNumber)
            : base($"The bank account number '{accountNumber}' is invalid.")
        {
        }
    }
}
