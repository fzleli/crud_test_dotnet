using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Domain.Entities;
using Mc2.CrudTest.Domain.Exceptions;
using MediatR;

namespace Mc2.CrudTest.Application.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            if (!PhoneNumberIsValid(request.PhoneNumber))
                throw new InvalidPhoneNumberException(request.PhoneNumber);

            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                BankAccountNumber = request.BankAccountNumber
            };

            await _customerRepository.AddAsync(customer);

            return new CustomerDto
            {
                FirstName = customer.FirstName
            };
        }

        private bool PhoneNumberIsValid(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone) && phone.StartsWith("+98");
        }
    }
}
