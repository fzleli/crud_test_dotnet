using Mc2.CrudTest.Domain.Common;

namespace Mc2.CrudTest.Application.Common.Interfaces;

public interface IEventStore
{
    Task SaveEventAsync(DomainEvent domainEvent);
}