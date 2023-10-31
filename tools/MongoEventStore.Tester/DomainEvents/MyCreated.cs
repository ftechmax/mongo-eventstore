using WebApplication1;

namespace MongoEventStore.Tester.DomainEvents;

public class MyCreated : DomainEvent
{
    public string Name { get; set; }

    public string Description { get; set; }

    private MyCreated()
    {
    }

    public MyCreated(MyAggregate aggregate)
        : this()
    {
        Id = aggregate.Id;
        TimeStamp = DateTimeOffset.UtcNow;
        Name = aggregate.Name;
        Description = aggregate.Description;
    }
}