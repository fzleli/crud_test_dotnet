using FluentAssertions;
using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Application.Customers.Commands.CreateCustomer;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Events;
using Mc2.CrudTest.Domain.Exceptions;
using Moq;

namespace Mc2.CrudTest.AcceptanceTests.Steps
{
    [Binding]
    public class CreateCustomerSteps
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
        private readonly Mock<IPhoneNumberValidator> _phoneValidatorMock = new();
        private readonly Mock<IEventStore> _eventStoreMock = new();
        private CreateCustomerCommand _command;
        private Exception _thrownException;
        private Customer _result;

        [Given(@"I have a valid customer creation request")]
        public void GivenIHaveAValidCustomerCreationRequest()
        {
            _command = new CreateCustomerCommand
            {
                FirstName = "Amir",
                LastName = "Moradi",
                DateOfBirth = new DateTime(1990, 5, 12),
                PhoneNumber = "+989123456789",
                Email = "test@example.com",
                BankAccountNumber = "8205401026800208"
            };
            _phoneValidatorMock.Setup(v => v.IsValid(_command.PhoneNumber)).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(_command.Email)).ReturnsAsync(true);
            _customerRepositoryMock.Setup(r => r.IsUniqueByNameAndBirthAsync(_command.FirstName, _command.LastName, _command.DateOfBirth)).ReturnsAsync(true);
        }

        [Given(@"I have a customer creation request with an invalid phone number")]
        public void GivenIHaveACustomerCreationRequestWithAnInvalidPhoneNumber()
        {
            _command = new CreateCustomerCommand { PhoneNumber = "123" };
            _phoneValidatorMock.Setup(v => v.IsValid(_command.PhoneNumber)).Returns(false);
        }

        [Given(@"I have a customer creation request with a duplicate email")]
        public void GivenIHaveACustomerCreationRequestWithADuplicateEmail()
        {
            _command = new CreateCustomerCommand { Email = "duplicate@example.com" };
            _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(_command.Email)).ReturnsAsync(false);
        }

        [Given(@"I have a customer creation request with duplicate name and date of birth")]
        public void GivenIHaveACustomerCreationRequestWithDuplicateNameAndDateOfBirth()
        {
            _command = new CreateCustomerCommand { FirstName = "Amir", LastName = "Moradi", DateOfBirth = DateTime.Now };
            _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
            _customerRepositoryMock.Setup(r => r.IsEmailUniqueAsync(It.IsAny<string>())).ReturnsAsync(true);
            _customerRepositoryMock.Setup(r => r.IsUniqueByNameAndBirthAsync(_command.FirstName, _command.LastName, _command.DateOfBirth)).ReturnsAsync(false);
        }

        [When(@"I send the create customer command")]
        public async Task WhenISendTheCreateCustomerCommand()
        {
            var handler = new CreateCustomerCommandHandler(_customerRepositoryMock.Object, _phoneValidatorMock.Object, _eventStoreMock.Object);
            try
            {
                _result = await handler.Handle(_command, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _thrownException = ex;
            }
        }

        [Then(@"the customer should be created successfully")]
        public void ThenTheCustomerShouldBeCreatedSuccessfully()
        {
            _result.Should().NotBeNull();
            _customerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Then(@"a CustomerCreatedEvent should be raised")]
        public void ThenACustomerCreatedEventShouldBeRaised()
        {
            _eventStoreMock.Verify(e => e.SaveEventAsync(It.IsAny<CustomerCreatedEvent>()), Times.Once);
        }

        [Then(@"an InvalidPhoneNumberException should be thrown")]
        public void ThenAnInvalidPhoneNumberExceptionShouldBeThrown()
        {
            _thrownException.Should().BeOfType<InvalidPhoneNumberException>();
        }

        [Then(@"a DuplicateEmailException should be thrown")]
        public void ThenADuplicateEmailExceptionShouldBeThrown()
        {
            _thrownException.Should().BeOfType<DuplicateEmailException>();
        }

        [Then(@"a DuplicateCustomerException should be thrown")]
        public void ThenADuplicateCustomerExceptionShouldBeThrown()
        {
            _thrownException.Should().BeOfType<DuplicateCustomerException>();
        }
    }
}
