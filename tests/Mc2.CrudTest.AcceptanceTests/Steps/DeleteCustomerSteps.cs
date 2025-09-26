using Application.Common.Exceptions;
using FluentAssertions;
using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Application.Customers.Commands.DeleteCustomer;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Events;
using Moq;

namespace Mc2.CrudTest.AcceptanceTests.Steps;

[Binding]
public class DeleteCustomerSteps
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IEventStore> _eventStoreMock = new();
    private DeleteCustomerCommand _command;
    private Customer _existingCustomer;
    private Exception _thrownException;

    [Given(@"a customer exists in the system with ID ""(.*)"" while deleting a customer")]
    public void GivenACustomerExists(Guid id)
    {
        _existingCustomer = Customer.Create(
            "Amir", "Moradi", new DateTime(1990, 5, 12),
            "+989123456789", "test@example.com", "8205401026800208");

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(_existingCustomer);
    }

    [Given(@"no customer exists in the system with ID ""(.*)"" while deleting a customer")]
    public void GivenNoCustomerExists(Guid id)
    {
        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Customer)null);
    }

    [Given(@"a customer exists in the system with ID ""(.*)"" and is already deleted")]
    public void GivenCustomerAlreadyDeleted(Guid id)
    {
        // شبیه‌سازی مشتری حذف شده
        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Customer)null);
    }

    [When(@"I send the delete customer command for ID ""(.*)""")]
    public async Task WhenISendDeleteCustomerCommand(Guid id)
    {
        _command = new DeleteCustomerCommand(id);

        var handler = new DeleteCustomerCommandHandler(
            _customerRepositoryMock.Object,
            _eventStoreMock.Object
        );

        try
        {
            await handler.Handle(_command, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _thrownException = ex;
        }
    }

    [Then(@"the customer should be removed successfully")]
    public void ThenCustomerRemovedSuccessfully()
    {
        _customerRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Then(@"a CustomerDeletedEvent should be raised")]
    public void ThenCustomerDeletedEventRaised()
    {
        _eventStoreMock.Verify(e => e.SaveEventAsync(It.IsAny<CustomerDeletedEvent>()), Times.Once);
    }

    [Then(@"a NotFoundException should be thrown while deleting a customer")]
    public void ThenNotFoundExceptionThrownWhileDeletingACustomer()
    {
        _thrownException.Should().BeOfType<NotFoundException>();
    }
}
