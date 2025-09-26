using FluentAssertions;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Events;

namespace Mc2.CrudTest.Domain.UnitTests.Customers
{
    public class CustomerTests
    {
        [Fact]
        public void Create_ShouldReturnCustomer_AndRaiseEvent_WhenAllValid()
        {
            // Arrange
            var firstName = "Amir";
            var lastName = "Moradi";
            var dob = new DateTime(1990, 5, 12);
            var phone = "+989123456789";
            var email = "test@example.com";
            var bankAccount = "8205401026800208";
            
            // Act
            var customer = Customer.Create(firstName, lastName, dob, phone, email, bankAccount);

            // Assert
            customer.FirstName.Should().Be(firstName);
            customer.LastName.Should().Be(lastName);
            customer.DateOfBirth.Should().Be(dob);

            var domainEvents = customer.DomainEvents;
            domainEvents.Should().HaveCount(1);
            domainEvents.First().Should().BeOfType<CustomerCreatedEvent>();

            var createdEvent = (CustomerCreatedEvent)domainEvents.First();
            createdEvent.Customer.Id.Should().Be(customer.Id);
            createdEvent.Customer.FirstName.Should().Be(firstName);
            createdEvent.Customer.LastName.Should().Be(lastName);
        }

        [Fact]
        public void Update_ShouldReturnCustomer_AndRaiseEvent_WhenAllValid()
        {
            // Arrange
            var firstName = "Amir";
            var lastName = "Moradi";
            var dob = new DateTime(1990, 5, 12);
            var phone = "+989123456789";
            var email = "test@example.com";
            var bankAccount = "8205401026800208";
            var customer = Customer.Create(firstName, lastName, dob, phone, email, bankAccount);

            var updatedFirstName = "Vahid";
            var updatedLastName = "Amiri";
            var updatedDob = new DateTime(2000, 8, 07);

            // Act
            customer.Update(updatedFirstName, updatedLastName, updatedDob, phone, email, bankAccount);

            // Assert
            customer.FirstName.Should().Be(updatedFirstName);
            customer.LastName.Should().Be(updatedLastName);
            customer.DateOfBirth.Should().Be(updatedDob);

            var domainEvents = customer.DomainEvents;
            domainEvents.Should().HaveCount(2);
            domainEvents.First().Should().BeOfType<CustomerCreatedEvent>();
            domainEvents.Last().Should().BeOfType<CustomerUpdatedEvent>();

            var updatedEvent = (CustomerUpdatedEvent)domainEvents.Last();
            updatedEvent.Customer.Id.Should().Be(customer.Id);
            updatedEvent.Customer.FirstName.Should().Be(updatedFirstName);
            updatedEvent.Customer.LastName.Should().Be(updatedLastName);
        }
    }
}
