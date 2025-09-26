using FluentAssertions;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Events;

namespace Mc2.CrudTest.Domain.UnitTests.Customers.Events;

public class CustomerUpdatedEventTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        // Arrange
        var sampleCustomer = Customer.Create(
            "TestFirstName",
            "TestLastName",
            new DateTime(1990, 1, 1),
            "+1234567890",
            "test@example.com",
            "1234567890123456"
        );

        // Act
        var eventObj = new CustomerUpdatedEvent(sampleCustomer);

        // Assert
        eventObj.Customer.Should().Be(sampleCustomer);
        eventObj.Customer.Id.Should().Be(sampleCustomer.Id);
    }
}
