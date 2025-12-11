using FluentAssertions;
using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Application.Customers.Commands.CreateCustomer;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Events;
using Mc2.CrudTest.Domain.Exceptions;
using Moq;

namespace Mc2.CrudTest.Application.UnitTests.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandHandlerTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IPhoneNumberValidator> _phoneValidatorMock;
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly CreateCustomerCommandHandler _handler;

        public CreateCustomerCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _phoneValidatorMock = new Mock<IPhoneNumberValidator>();
            _eventStoreMock = new Mock<IEventStore>();
            _handler = new CreateCustomerCommandHandler(_customerRepositoryMock.Object, _phoneValidatorMock.Object, _eventStoreMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateCustomer_AndSaveEvent_WhenRequestIsValid()
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                FirstName = "Amir",
                LastName = "Moradi",
                DateOfBirth = new DateTime(1990, 5, 12),
                PhoneNumber = "+989123456789",
                Email = "test@example.com",
                BankAccountNumber = "8205401026800208"
            };

            _phoneValidatorMock.Setup(v => v.IsValid(command.PhoneNumber)).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(command.Email)).ReturnsAsync(true);
            _customerRepositoryMock.Setup(r => r.IsUniqueByNameAndBirthAsync(command.FirstName, command.LastName, command.DateOfBirth)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _customerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
            _eventStoreMock.Verify(e => e.SaveEventAsync(It.IsAny<CustomerCreatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidPhoneNumberException_WhenPhoneNumberIsInvalid()
        {
            // Arrange
            var command = new CreateCustomerCommand { PhoneNumber = "123" };
            _phoneValidatorMock.Setup(v => v.IsValid(command.PhoneNumber)).Returns(false);

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidPhoneNumberException>();
            _customerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowDuplicateEmailException_WhenEmailExists()
        {
            // Arrange
            var command = new CreateCustomerCommand { Email = "duplicate@example.com" };
            _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(command.Email)).ReturnsAsync(false);

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DuplicateEmailException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowDuplicateCustomerException_WhenPersonalInfoExists()
        {
            // Arrange
            var command = new CreateCustomerCommand { FirstName = "Amir", LastName = "Moradi", DateOfBirth = DateTime.Now };
            _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(It.IsAny<string>())).ReturnsAsync(true);
            _customerRepositoryMock.Setup(r => r.IsUniqueByNameAndBirthAsync(command.FirstName, command.LastName, command.DateOfBirth)).ReturnsAsync(false);

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DuplicateCustomerException>();
        }
    }
}
