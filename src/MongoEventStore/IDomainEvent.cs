namespace MongoEventStore;

public interface IDomainEvent
{
    /// <summary>
    /// The aggregate id
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// The timestamp this event occurred
    /// </summary>
    DateTimeOffset TimeStamp { get; set; }
}