using Microsoft.AspNetCore.Mvc;
using MongoEventStore;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IEventStore _eventStore;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IEventStore eventStore)
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

    public class CreateDto
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class UpdateDto
    {
        public string Description { get; set; }
    }
}