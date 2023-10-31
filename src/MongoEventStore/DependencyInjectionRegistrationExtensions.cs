using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace MongoEventStore;

public static class DependencyInjectionRegistrationExtensions
{
    public static IServiceCollection AddMongoEventStore(this IServiceCollection collection, Action<EventStoreConfig>? config = null)
    {
        BsonClassMap.RegisterClassMap<EventData>();
        BsonClassMap.RegisterClassMap<DomainEvent>(i =>
        {
            i.AutoMap();
            i.SetIsRootClass(true);
        });

        var eventStoreConfig = new EventStoreConfig();
        config?.Invoke(eventStoreConfig);

        collection.AddScoped<IEventStore, EventStore>();

        collection.AddSingleton(eventStoreConfig);

        return collection;
    }
}
