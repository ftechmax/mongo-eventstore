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

        collection.AddScoped<IEventStore, EventStore>();
        //collection.AddSingleton(new EventStoreConfig(databaseName));

        if (config != null)
        {
            collection.Configure(config);
        }

        return collection;
    }
}

////public static class DependencyInjectionRegistrationExtensions
////{
////    public static IServiceCollection AddMongoEventStore(this IServiceCollection collection, Action<IBusRegistrationConfigurator> configure = null)
////    {
////        if (collection.Any(d => d.ServiceType == typeof(IBus)))
////        {
////            throw new ConfigurationException(
////                "AddMassTransit() was already called and may only be called once per container. To configure additional bus instances, refer to the documentation: https://masstransit-project.com/usage/containers/multibus.html");
////        }

////        AddInstrumentation(collection);

////        var configurator = new ServiceCollectionBusConfigurator(collection);

////        configure?.Invoke(configurator);

////        configurator.Complete();

////        return collection;
////    }
////}

////public interface IRegistrationConfigurator :
////    IServiceCollection
////{
////    /// <summary>
////    /// Adds the consumer, allowing configuration when it is configured on an endpoint
////    /// </summary>
////    /// <param name="configure"></param>
////    /// <typeparam name="T">The consumer type</typeparam>
////    IConsumerRegistrationConfigurator<T> AddConsumer<T>(Action<IRegistrationContext, IConsumerConfigurator<T>> configure = null)
////        where T : class, IConsumer;
////}