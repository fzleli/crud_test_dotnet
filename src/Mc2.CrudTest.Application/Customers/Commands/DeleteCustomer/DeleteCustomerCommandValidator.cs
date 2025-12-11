using FluentValidation;
using Mc2.CrudTest.Application.Common.Interfaces;

namespace Mc2.CrudTest.Application.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator(IPhoneNumberValidator phoneNumberValidator)
    {
        RuleFor(c => c.Id)
            .NotNull().WithMessage("Customer id is required");
    }
}
