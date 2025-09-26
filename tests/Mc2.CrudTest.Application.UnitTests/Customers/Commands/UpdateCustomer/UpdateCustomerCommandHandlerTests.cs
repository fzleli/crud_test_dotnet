using Application.Common.Exceptions;
using FluentAssertions;
using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Application.Customers.Commands.UpdateCustomer;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Events;
using Mc2.CrudTest.Domain.Exceptions;
using Moq;

namespace Mc2.CrudTest.Application.UnitTests.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandlerTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IPhoneNumberValidator> _phoneValidatorMock;
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly UpdateCustomerCommandHandler _handler;

        public UpdateCustomerCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _phoneValidatorMock = new Mock<IPhoneNumberValidator>();
            _eventStoreMock = new Mock<IEventStore>();
            _handler = new UpdateCustomerCommandHandler(
                _customerRepositoryMock.Object,
                _phoneValidatorMock.Object,
                _eventStoreMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldUpdateCustomer_AndSaveEvent_WhenRequestIsValid()
        {
            // Arrange
            var existingCustomer = Customer.Create("John", "Doe", DateTime.Now.AddYears(-30),
                "+9182457594", "john@example.com", "1234567895479652");

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingCustomer);
            _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(true);
            _customerRepositoryMock.Setup(r => r.IsUniqueByNameAndBirthAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).ReturnsAsync(true);

            var command = new UpdateCustomerCommand
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                Email = "jane@example.com"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.FirstName.Should().Be("Jane");
            result.Email.Value.Should().Be("jane@example.com");
            _customerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Once);
            _eventStoreMock.Verify(e => e.SaveEventAsync(It.IsAny<CustomerUpdatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidPhoneNumberException_WhenPhoneNumberIsInvalid()
        {
            // Arrange
            var customer = Customer.Create("John", "Doe", DateTime.Now.AddYears(-30), "+9182654785", "john@example.com", "1234567895479652");

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(customer);
            _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(false);

            var command = new UpdateCustomerCommand
            {
                Id = Guid.NewGuid(),
                PhoneNumber = "invalid"
            };

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidPhoneNumberException>();
            _customerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowDuplicateEmailException_WhenEmailExists()
        {
            // Arrange
            var customer = Customer.Create("John", "Doe", DateTime.Now.AddYears(-30), "+9182654785", "john@example.com", "1234567895479652");

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(customer);
            _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(false);

            var command = new UpdateCustomerCommand
            {
                Id = Guid.NewGuid(),
                Email = "duplicate@example.com"
            };

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DuplicateEmailException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowDuplicateCustomerException_WhenPersonalInfoExists()
        {
            // Arrange
            var customer = Customer.Create("John", "Doe", DateTime.Now.AddYears(-30), "+9182654785", "john@example.com", "1234567895479652");

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(customer);
            _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(true);
            _customerRepositoryMock.Setup(r => r.IsUniqueByNameAndBirthAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<Guid>())).ReturnsAsync(false);

            var command = new UpdateCustomerCommand
            {
                Id = Guid.NewGuid(),
                FirstName = "Changed",
                LastName = "Changed"
            };

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DuplicateCustomerException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenCustomerDoesNotExist()
        {
            // Arrange
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Customer)null!);

            var command = new UpdateCustomerCommand { Id = Guid.NewGuid() };

            // Act
            Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
