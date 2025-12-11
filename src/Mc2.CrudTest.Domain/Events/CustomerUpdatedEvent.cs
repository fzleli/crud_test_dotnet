using Mc2.CrudTest.Domain.Common;
using Mc2.CrudTest.Domain.Entities;

namespace Mc2.CrudTest.Domain.Events;

public class CustomerUpdatedEvent : DomainEvent
{
    public Customer Customer { get; }

    public CustomerUpdatedEvent(Customer customer) => Customer = customer;
}
