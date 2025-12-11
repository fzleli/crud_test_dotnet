using FluentValidation;
using Mc2.CrudTest.Application.Common.Interfaces;

namespace Mc2.CrudTest.Application.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator(IPhoneNumberValidator phoneNumberValidator)
        {
            RuleFor(c => c.FirstName)
                .NotEmpty().WithMessage("First name is required");

            RuleFor(c => c.LastName)
                .NotEmpty().WithMessage("Last name is required");

            RuleFor(c => c.DateOfBirth)
                .Must(dob => dob.Date <= DateTime.UtcNow.Date)
                .WithMessage("Date of birth cannot be in the future");

            RuleFor(c => c.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Must(phoneNumberValidator.IsValid)
                .WithMessage("Invalid phone number format");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(c => c.BankAccountNumber)
                .NotEmpty().WithMessage("Bank account number is required");
        }
    }
}
