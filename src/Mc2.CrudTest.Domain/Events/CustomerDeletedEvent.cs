using Mc2.CrudTest.Domain.Common;
using Mc2.CrudTest.Domain.Entities;

namespace Mc2.CrudTest.Domain.Events;

public class CustomerDeletedEvent : DomainEvent
{
    public Customer Customer { get; }

    public CustomerDeletedEvent(Customer customer) => Customer = customer;
}
