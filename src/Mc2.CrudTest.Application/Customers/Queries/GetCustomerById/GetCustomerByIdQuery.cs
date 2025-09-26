using Mc2.CrudTest.Domain.Entities;
using MediatR;

namespace Mc2.CrudTest.Application.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<Customer>;
