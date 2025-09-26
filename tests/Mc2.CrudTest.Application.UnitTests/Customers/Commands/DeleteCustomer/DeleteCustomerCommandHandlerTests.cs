using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Application.Customers.Commands.DeleteCustomer;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Events;
using Moq;

namespace Mc2.CrudTest.Application.UnitTests.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IEventStore> _eventStoreMock;
    private readonly DeleteCustomerCommandHandler _handler;

    public DeleteCustomerCommandHandlerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _eventStoreMock = new Mock<IEventStore>();
        _handler = new DeleteCustomerCommandHandler(_customerRepositoryMock.Object, _eventStoreMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteCustomer_AndRaiseEvent_WhenCustomerExists()
    {
        // Arrange
        var existingCustomer = Customer.Create("John", "Doe", DateTime.Now.AddYears(-30),
            "+9182457594", "john@example.com", "1234567895479652");

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingCustomer);

        var command = new DeleteCustomerCommand(Guid.NewGuid());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _customerRepositoryMock.Verify(r => r.DeleteAsync(existingCustomer), Times.Once);
        _eventStoreMock.Verify(e => e.SaveEventAsync(It.IsAny<CustomerDeletedEvent>()), Times.Once);
    }
}
