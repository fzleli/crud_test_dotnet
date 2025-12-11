using Mc2.CrudTest.Domain.Entities;
using MediatR;

namespace Mc2.CrudTest.Application.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery : IRequest<IEnumerable<Customer>>;
