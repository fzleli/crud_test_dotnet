using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Exceptions;
using MediatR;

namespace Mc2.CrudTest.Application.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Customer>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IPhoneNumberValidator _phoneNumberValidator;
        private readonly IEventStore _eventStore;
        public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IPhoneNumberValidator phoneNumberValidator, IEventStore eventStore)
        {
            _customerRepository = customerRepository;
            _phoneNumberValidator = phoneNumberValidator;
            _eventStore = eventStore;
        }

        public async Task<Customer> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            if (!_phoneNumberValidator.IsValid(request.PhoneNumber))
                throw new InvalidPhoneNumberException(request.PhoneNumber);

            if (!await _customerRepository.IsEmailUniqueAsync(request.Email))
                throw new DuplicateEmailException(request.Email);

            if (!await _customerRepository.IsUniqueByNameAndBirthAsync(request.FirstName, request.LastName, request.DateOfBirth))
                throw new DuplicateCustomerException();

            var customer = Customer.Create(request.FirstName, request.LastName, request.DateOfBirth, request.PhoneNumber, request.Email, request.BankAccountNumber);

            await _customerRepository.AddAsync(customer);

            foreach (var domainEvent in customer.DomainEvents)
            {
                await _eventStore.SaveEventAsync(domainEvent);
            }

            customer.ClearDomainEvents();

            return customer;
        }
    }
}
