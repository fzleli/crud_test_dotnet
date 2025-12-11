namespace Mc2.CrudTest.Infrastructure.Persistence.Entity;

public class DomainEventEntity
{
    public Guid Id { get; set; }
    public string? Type { get; set; }
    public string? Data { get; set; }
    public DateTime OccurredOn { get; set; }
}
