namespace Mc2.CrudTest.Domain.Exceptions
{
    public class InvalidEmailException : Exception
    {
        public InvalidEmailException(string email)
            : base($"The email '{email}' is invalid.")
        {
        }
    }
}
