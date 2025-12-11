using MediatR;

namespace Mc2.CrudTest.Application.Customers.Commands.DeleteCustomer;

public record DeleteCustomerCommand(Guid Id) : IRequest<Unit>;

