using WebApplication1;

namespace MongoEventStore.Tester.DomainEvents;

public class MyUpdated : DomainEvent
{
    public string Description { get; set; }

    public MyUpdated(MyAggregate aggregate)
    {
        Id = aggregate.Id;
        TimeStamp = DateTimeOffset.UtcNow;
        Description = aggregate.Description;
    }
}