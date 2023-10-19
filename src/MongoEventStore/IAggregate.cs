namespace MongoEventStore;

public interface IAggregate
{
    Guid Id { get; }
}