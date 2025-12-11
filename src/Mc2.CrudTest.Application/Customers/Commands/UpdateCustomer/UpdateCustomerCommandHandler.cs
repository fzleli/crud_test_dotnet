using Application.Common.Exceptions;
using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Exceptions;
using MediatR;

namespace Mc2.CrudTest.Application.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Customer>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IPhoneNumberValidator _phoneNumberValidator;
        private readonly IEventStore _eventStore;

        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IPhoneNumberValidator phoneNumberValidator, IEventStore eventStore)
        {
            _customerRepository = customerRepository;
            _phoneNumberValidator = phoneNumberValidator;
            _eventStore = eventStore;
        }

        public async Task<Customer> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.Id);

            if (customer == null)
                throw new NotFoundException(nameof(Customer), request.Id);

            if (!_phoneNumberValidator.IsValid(request.PhoneNumber))
                throw new InvalidPhoneNumberException(request.PhoneNumber);

            if (!await _customerRepository.IsEmailUniqueAsync(request.Email, customer.Id))
                throw new DuplicateEmailException(request.Email);

            if (!await _customerRepository.IsUniqueByNameAndBirthAsync(request.FirstName, request.LastName, request.DateOfBirth, customer.Id))
                throw new DuplicateCustomerException();

            customer.Update(request.FirstName, request.LastName, request.DateOfBirth, request.PhoneNumber, request.Email, request.BankAccountNumber);

            await _customerRepository.UpdateAsync(customer);

            foreach (var domainEvent in customer.DomainEvents)
                await _eventStore.SaveEventAsync(domainEvent);

            customer.ClearDomainEvents();

            return customer;
        }
    }
}
