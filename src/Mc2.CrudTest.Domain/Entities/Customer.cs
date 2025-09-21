using Mc2.CrudTest.Domain.ValueObjects;

namespace Mc2.CrudTest.Domain.Entities
{
    public class Customer
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public Email Email { get; private set; }
        public BankAccountNumber BankAccountNumber { get; private set; }

        private Customer(
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            PhoneNumber phoneNumber,
            Email email,
            BankAccountNumber bankAccountNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            Email = email;
            BankAccountNumber = bankAccountNumber;
        }

        public static Customer Create(
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            string phoneNumberValue,
            string emailValue,
            string bankAccountNumberValue)
        {
            var phoneNumber = PhoneNumber.Create(phoneNumberValue);
            var email = Email.Create(emailValue);
            var bankAccountNumber = BankAccountNumber.Create(bankAccountNumberValue);

            return new Customer(firstName, lastName, dateOfBirth, phoneNumber, email, bankAccountNumber);
        }

        public void UpdatePhoneNumber(string newPhoneNumberValue)
        {
            PhoneNumber = PhoneNumber.Create(newPhoneNumberValue);
        }
    }
}
