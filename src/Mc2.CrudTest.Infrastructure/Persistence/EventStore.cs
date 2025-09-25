using Mc2.CrudTest.Application.Common.Interfaces;
using Mc2.CrudTest.Domain.Common;
using Mc2.CrudTest.Infrastructure.Persistence.Entity;
using System.Text.Json;

namespace Mc2.CrudTest.Infrastructure.Persistence;

public class EventStore : IEventStore
{
    private readonly AppDbContext _context;

    public EventStore(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveEventAsync(DomainEvent domainEvent)
    {
        var eventEntity = new DomainEventEntity
        {
            Id = Guid.NewGuid(),
            Type = domainEvent.GetType().AssemblyQualifiedName,
            Data = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
            OccurredOn = domainEvent.OccurredOn
        };

        _context.DomainEvents.Add(eventEntity);
        await _context.SaveChangesAsync();
    }
}
