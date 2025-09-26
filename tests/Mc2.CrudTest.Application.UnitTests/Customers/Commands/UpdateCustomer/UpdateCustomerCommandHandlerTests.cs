using FluentAssertions;
using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Application.Customers.Commands.UpdateCustomer;
using Mc2.CrudTest.Domain.Common;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Exceptions;
using Moq;

namespace Mc2.CrudTest.Application.UnitTests.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandlerTests
    {
        private Mock<ICustomerRepository> _mockRepo;
        private Mock<IPhoneNumberValidator> _mockPhoneValidator;
        private Mock<IEventStore> _mockEventStore;
        private UpdateCustomerCommandHandler _handler;

        public UpdateCustomerCommandHandlerTests()
        {
            _mockRepo = new Mock<ICustomerRepository>();
            _mockPhoneValidator = new Mock<IPhoneNumberValidator>();
            _mockEventStore = new Mock<IEventStore>();
            _handler = new UpdateCustomerCommandHandler(_mockRepo.Object, _mockPhoneValidator.Object, _mockEventStore.Object);
        }

        [Fact]
        public async Task Handle_ValidUpdate_UpdatesCustomerAndSavesEvents()
        {
            // Arrange
            var existingCustomer = Customer.Create("John", "Doe", DateTime.Now.AddYears(-30), "+1234567890", "john@example.com", "123456789");
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(existingCustomer);
            _mockPhoneValidator.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
            _mockRepo.Setup(r => r.IsEmailUniqueAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockRepo.Setup(r => r.IsUniqueByNameAndBirthAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(true);

            var command = new UpdateCustomerCommand { Id = Guid.NewGuid(), FirstName = "Jane", Email = "jane@example.com" };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.FirstName.Should().Be("Jane");
            result.Email.Value.Should().Be("jane@example.com");
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Once);
            _mockEventStore.Verify(e => e.SaveEventAsync(It.IsAny<DomainEvent>()), Times.AtLeastOnce);
        }

        [Fact]
        public void Handle_InvalidPhoneNumber_ThrowsInvalidPhoneNumberException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(Customer.Create());
            _mockPhoneValidator.Setup(v => v.IsValid(It.IsAny<string>())).Returns(false);

            var command = new UpdateCustomerCommand { Id = Guid.NewGuid(), PhoneNumber = "invalid" };

            // Act & Assert
            Assert.ThrowsAsync<InvalidPhoneNumberException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}