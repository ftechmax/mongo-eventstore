namespace MongoEventStore;

public interface IEventStore
{
    Task<T> GetAsync<T>(Guid id) where T : IAggregate;

    Task SaveAsync(IDomainEvent domainEvent);

    Task SaveAsync(IEnumerable<IDomainEvent> domainEvents);
}