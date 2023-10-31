using Microsoft.AspNetCore.Mvc;
using MongoEventStore.Tester.DomainEvents;
using MongoEventStore.Tester.Dtos;
using WebApplication1;

namespace MongoEventStore.Tester.Controllers;

[ApiController]
[Route("[controller]")]
public class EventStoreController : ControllerBase
{
    private readonly ILogger<EventStoreController> _logger;
    private readonly IEventStore _eventStore;

    public EventStoreController(ILogger<EventStoreController> logger, IEventStore eventStore)
    {
        _logger = logger;
        _eventStore = eventStore;
    }

    [HttpGet("{id}")]
    public async Task<MyAggregate> Get(Guid id)
    {
        var aggregate = await _eventStore.GetAsync<MyAggregate>(id);

        return aggregate;
    }

    [HttpPost]
    public async Task Post([FromBody] CreateDto dto)
    {
        var aggregate = new MyAggregate(dto);
        var domainEvent = new MyCreated(aggregate);

        await _eventStore.SaveAsync(domainEvent);
    }

    [HttpPut("{id}")]
    public async Task Put(Guid id, [FromBody] UpdateDto dto)
    {
        var aggregate = await _eventStore.GetAsync<MyAggregate>(id);

        var domainEvent = aggregate.Update(dto);

        await _eventStore.SaveAsync(domainEvent);
    }
}