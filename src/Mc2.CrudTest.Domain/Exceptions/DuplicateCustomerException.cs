namespace Mc2.CrudTest.Domain.Exceptions
{
    public class DuplicateCustomerException : Exception
    {
        public DuplicateCustomerException(string firstName, string lastName, DateTime dateOfBirth)
        : base($"A customer with FirstName '{firstName}', LastName '{lastName}', and DateOfBirth '{dateOfBirth:yyyy-MM-dd}' already exists.")
        {
        }

        public DuplicateCustomerException() : base("Duplicate customer based on name and date of birth.") { }
    }
}
