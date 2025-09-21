using FluentAssertions;
using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Application.Customers.Commands.CreateCustomer;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Exceptions;
using Moq;

public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IPhoneNumberValidator> _phoneValidatorMock;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _phoneValidatorMock = new Mock<IPhoneNumberValidator>();
        _handler = new CreateCustomerCommandHandler(_customerRepositoryMock.Object, _phoneValidatorMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateCustomer_WhenRequestIsValid()
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            FirstName = "Amir",
            LastName = "Moradi",
            DateOfBirth = new DateTime(1990, 5, 12),
            PhoneNumber = "+989123456789",
            Email = "test@example.com",
            BankAccountNumber = "IR820540102680020817909002"
        };

        _phoneValidatorMock.Setup(v => v.IsValid(command.PhoneNumber)).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(command.FirstName);
        _customerRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Customer>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenPhoneNumberIsInvalid()
    {
        // Arrange
        var command = new CreateCustomerCommand { PhoneNumber = "123" };

        _phoneValidatorMock.Setup(v => v.IsValid(command.PhoneNumber)).Returns(false);

        // Act
        var func = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await func.Should().ThrowAsync<InvalidPhoneNumberException>();
    }
}
