using Mc2.CrudTest.Domain.Common;
using Mc2.CrudTest.Domain.Events;
using Mc2.CrudTest.Domain.ValueObjects;

namespace Mc2.CrudTest.Domain.Entities;

public class Customer : AggregateRoot<Guid>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Email Email { get; private set; }
    public BankAccountNumber BankAccountNumber { get; private set; }

    private Customer() { }

    public static Customer Create(string firstName, string lastName, DateTime dateOfBirth, string phoneNumber, string email, string bankAccountNumber)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth.Date,
            PhoneNumber = PhoneNumber.Create(phoneNumber),
            Email = Email.Create(email),
            BankAccountNumber = BankAccountNumber.Create(bankAccountNumber)
        };

        customer.AddDomainEvent(new CustomerCreatedEvent(customer));

        return customer;
    }

    public void Update(string firstName, string lastName, DateTime dateOfBirth, string phoneNumber, string email, string bankAccountNumber)
    {
        if (firstName is not null)
            FirstName = firstName;

        if (lastName is not null)
            LastName = lastName;

        if (dateOfBirth != default)
            DateOfBirth = dateOfBirth;

        if (phoneNumber is not null)
            PhoneNumber = PhoneNumber.Create(phoneNumber);

        if (email is not null)
            Email = Email.Create(email);

        if (bankAccountNumber is not null)
            BankAccountNumber = BankAccountNumber.Create(bankAccountNumber);

        AddDomainEvent(new CustomerUpdatedEvent(this));
    }
}
