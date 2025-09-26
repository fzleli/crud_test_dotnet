using Application.Common.Exceptions;
using FluentAssertions;
using Mc2.CrudTest.Application.Customers.Queries.GetCustomerById;
using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Domain.Entities;
using Moq;

namespace Mc2.CrudTest.Application.UnitTests.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly GetCustomerByIdQueryHandler _handler;

    public GetCustomerByIdQueryHandlerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _handler = new GetCustomerByIdQueryHandler(_customerRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCustomer_WhenCustomerExists()
    {
        // Arrange
        var existingCustomer = Customer.Create("John", "Doe", DateTime.Now.AddYears(-30),
            "+9182457594", "john@example.com", "1234567895479652");

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingCustomer);

        var query = new GetCustomerByIdQuery(existingCustomer.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(existingCustomer);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenCustomerDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Customer)null);

        var query = new GetCustomerByIdQuery(id);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}