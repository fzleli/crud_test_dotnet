using Mc2.CrudTest.Domain.Common;
using Mc2.CrudTest.Domain.Entities;

namespace Mc2.CrudTest.Domain.Events
{
    public class CustomerCreatedEvent : DomainEvent
    {
        public Customer Customer { get; }

        public CustomerCreatedEvent(Customer customer) => Customer = customer;
    }
}
