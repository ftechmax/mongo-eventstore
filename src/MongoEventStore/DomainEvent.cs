namespace MongoEventStore;

public abstract class DomainEvent : IDomainEvent
{
    public Guid Id { get; set; }

    public DateTimeOffset TimeStamp { get; set; }
}