using Application.Common.Exceptions;
using FluentAssertions;
using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Application.Customers.Commands.UpdateCustomer;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Events;
using Mc2.CrudTest.Domain.Exceptions;
using Moq;

namespace Mc2.CrudTest.AcceptanceTests.Steps
{
    [Binding]
    public class UpdateCustomerSteps
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
        private readonly Mock<IPhoneNumberValidator> _phoneValidatorMock = new();
        private readonly Mock<IEventStore> _eventStoreMock = new();
        private UpdateCustomerCommand _command;
        private Exception _thrownException;
        private Customer _existingCustomer;
        private Customer _result;

        [Given(@"I have an existing customer with valid data")]
        public void GivenIHaveAnExistingCustomerWithValidData()
        {
            _existingCustomer = Customer.Create(
                "Amir", "Moradi", new DateTime(1990, 5, 12),
                "+989123456789", "test@example.com", "8205401026800208");

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(_existingCustomer.Id))
                .ReturnsAsync(_existingCustomer);
        }

        [Given(@"I have a valid update request")]
        public void GivenIHaveAValidUpdateRequest()
        {
            _command = new UpdateCustomerCommand
            {
                Id = _existingCustomer.Id,
                FirstName = "Vahid",
                LastName = "Amiri",
                DateOfBirth = new DateTime(2000, 8, 7),
                PhoneNumber = "+989121234567",
                Email = "new@example.com",
                BankAccountNumber = "8205401026800209"
            };

            _phoneValidatorMock.Setup(v => v.IsValid(_command.PhoneNumber)).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(_command.Email, _existingCustomer.Id))
                .ReturnsAsync(true);
            _customerRepositoryMock.Setup(r => r.IsUniqueByNameAndBirthAsync(
                _command.FirstName, _command.LastName, _command.DateOfBirth, _existingCustomer.Id))
                .ReturnsAsync(true);
        }

        [Given(@"I have an update request with an invalid phone number")]
        public void GivenIHaveAnUpdateRequestWithAnInvalidPhoneNumber()
        {
            _command = new UpdateCustomerCommand
            {
                Id = _existingCustomer.Id,
                PhoneNumber = "123"
            };
            _phoneValidatorMock.Setup(v => v.IsValid(_command.PhoneNumber)).Returns(false);
        }

        [Given(@"I have an update request with a duplicate email")]
        public void GivenIHaveAnUpdateRequestWithADuplicateEmail()
        {
            _command = new UpdateCustomerCommand
            {
                Id = _existingCustomer.Id,
                Email = "duplicate@example.com"
            };
            _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(_command.Email, _existingCustomer.Id))
                .ReturnsAsync(false);
        }

        [Given(@"I have an update request with duplicate name and date of birth")]
        public void GivenIHaveAnUpdateRequestWithDuplicateNameAndDateOfBirth()
        {
            _command = new UpdateCustomerCommand
            {
                Id = _existingCustomer.Id,
                FirstName = "Amir",
                LastName = "Moradi",
                DateOfBirth = new DateTime(1990, 5, 12)
            };
            _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(It.IsAny<string>(), _existingCustomer.Id))
                .ReturnsAsync(true);
            _customerRepositoryMock.Setup(r => r.IsUniqueByNameAndBirthAsync(
                _command.FirstName, _command.LastName, _command.DateOfBirth, _existingCustomer.Id))
                .ReturnsAsync(false);
        }

        [Given(@"I have an update request for a non-existent customer")]
        public void GivenIHaveAnUpdateRequestForANon_ExistentCustomer()
        {
            _command = new UpdateCustomerCommand { Id = Guid.NewGuid() };
            _customerRepositoryMock.Setup(r => r.GetByIdAsync(_command.Id))
                .ReturnsAsync((Customer)null);
        }

        [When(@"I send the update customer command")]
        public async Task WhenISendTheUpdateCustomerCommand()
        {
            var handler = new UpdateCustomerCommandHandler(
                _customerRepositoryMock.Object, _phoneValidatorMock.Object, _eventStoreMock.Object);

            try
            {
                _result = await handler.Handle(_command, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        [Then(@"the customer should be updated successfully")]
        public void ThenTheCustomerShouldBeUpdatedSuccessfully()
        {
            _result.Should().NotBeNull();
            _customerRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Then(@"a CustomerUpdatedEvent should be raised")]
        public void ThenACustomerUpdatedEventShouldBeRaised()
        {
            _eventStoreMock.Verify(e => e.SaveEventAsync(It.IsAny<CustomerUpdatedEvent>()), Times.Once);
        }

        [Then(@"an InvalidPhoneNumberException should be thrown while updating a customer")]
        public void ThenAnInvalidPhoneNumberExceptionShouldBeThrownWhileUpdatingACustomer()
        {
            _thrownException.Should().BeOfType<InvalidPhoneNumberException>();
        }

        [Then(@"a DuplicateEmailException should be thrown while updating a customer")]
        public void ThenADuplicateEmailExceptionShouldBeThrownWhileUpdatingACustomer()
        {
            _thrownException.Should().BeOfType<DuplicateEmailException>();
        }

        [Then(@"a DuplicateCustomerException should be thrown while updating a customer")]
        public void ThenADuplicateCustomerExceptionShouldBeThrownWhileUpdatingACustomer()
        {
            _thrownException.Should().BeOfType<DuplicateCustomerException>();
        }

        [Then(@"a NotFoundException should be thrown")]
        public void ThenANotFoundExceptionShouldBeThrown()
        {
            _thrownException.Should().BeOfType<NotFoundException>();
        }
    }
}
