using Application.Common.Exceptions;
using FluentAssertions;
using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Application.Customers.Queries.GetCustomerById;
using Mc2.CrudTest.Domain.Common;
using Mc2.CrudTest.Domain.Entities;
using Moq;

namespace Mc2.CrudTest.AcceptanceTests.Steps;

[Binding]
public class GetCustomerByIdSteps
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IEventStore> _eventStoreMock = new();
    private Customer _existingCustomer;
    private object _queryResult;
    private Exception _thrownException;

    [Given(@"a customer exists in the system with ID ""(.*)""")]
    public void GivenCustomerExists(Guid id)
    {
        _existingCustomer = Customer.Create(
            "Amir", "Moradi", new DateTime(1990, 5, 12),
            "+989123456789", "test@example.com", "8205401026800208");

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(_existingCustomer);
    }

    [Given(@"no customer exists in the system with ID ""(.*)""")]
    public void GivenNoCustomerExists(Guid id)
    {
        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Customer)null);
    }

    [When(@"I send the get customer query for ID ""(.*)""")]
    public async Task WhenSendGetCustomerQuery(Guid id)
    {
        var query = new GetCustomerByIdQuery(id);
        var handler = new GetCustomerByIdQueryHandler(_customerRepositoryMock.Object);

        try
        {
            _queryResult = await handler.Handle(query, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _thrownException = ex;
        }
    }

    [Then(@"the customer details should be returned")]
    public void ThenCustomerDetailsShouldBeReturned()
    {
        _queryResult.Should().BeEquivalentTo(_existingCustomer);
    }

    [Then(@"a NotFoundException should be thrown for get customer")]
    public void ThenNotFoundExceptionThrownForGetCustomer()
    {
        _thrownException.Should().BeOfType<NotFoundException>();
    }

    [Then(@"no event should be saved")]
    public void ThenNoEventShouldBeSaved()
    {
        _eventStoreMock.Verify(e => e.SaveEventAsync(It.IsAny<DomainEvent>()), Times.Never);
    }
}
