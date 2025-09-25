namespace Mc2.CrudTest.Domain.Common;

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
