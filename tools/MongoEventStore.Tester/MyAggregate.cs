using MongoEventStore;
using MongoEventStore.Tester.DomainEvents;
using MongoEventStore.Tester.Dtos;

namespace WebApplication1
{
    public class MyAggregate : IAggregate
    {
        private MyAggregate()
        { }

        public MyAggregate(CreateDto dto)
            : this()
        {
            Id = Guid.NewGuid();
            Name = dto.Name;
            Description = dto.Description;
        }

        public MyUpdated Update(UpdateDto dto)
        {
            Description = dto.Description;
            return new MyUpdated(this);
        }

        private void On(MyCreated domainEvent)
        {
            Id = domainEvent.Id;
            Name = domainEvent.Name;
            Description = domainEvent.Description;
        }

        private void On(MyUpdated domainEvent)
        {
            Description = domainEvent.Description;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }
    }
}
