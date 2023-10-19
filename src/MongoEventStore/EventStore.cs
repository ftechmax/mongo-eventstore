using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MongoDB.Driver;

namespace MongoEventStore;

[ExcludeFromCodeCoverage]
internal class EventStore : IEventStore
{
    private readonly IMongoDatabase _mongoDatabase;

    private readonly IMongoCollection<EventData> _collection;

    public EventStore(IMongoClient mongoClient, EventStoreConfig config)
    {
        _mongoDatabase = mongoClient.GetDatabase(config.DatabaseName);
        _collection = GetCollection<EventData>();
    }

    public async Task<T> GetAsync<T>(Guid id) where T : IAggregate
    {
        var filterBuilder = Builders<EventData>.Filter;
        var filter = filterBuilder.Eq(EventData.StreamIdFieldName, id) &
                     filterBuilder.Gte(EventData.VersionFieldName, 0);

        var cursor = await _collection.FindAsync(filter);

        // TODO: ftechmax: disposes the cursor
        //if (!cursor.Any())
        //{
        //    return default;
        //}

        var type = typeof(T);
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        var instance = (T)type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!
            .Invoke(null);

        await cursor.ForEachAsync(i =>
        {
            var method = type.GetMethod("On", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { i.PayLoad.GetType() }, null);
            method?.Invoke(instance, new object[] { i.PayLoad });
        });

#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

        return instance;
    }

    public async Task SaveAsync(IDomainEvent domainEvent)
    {
        using var session = await _mongoDatabase.Client.StartSessionAsync();
        var transactionOptions = new TransactionOptions(ReadConcern.Snapshot, ReadPreference.Primary, WriteConcern.WMajority);
        session.StartTransaction(transactionOptions);
        try
        {
            var eventData = new EventData(domainEvent, await GetNextSequence(domainEvent.Id));
            await _collection.InsertOneAsync(eventData);

            await session.CommitTransactionAsync().ConfigureAwait(false);
        }
        catch
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    public async Task SaveAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        if (!domainEvents.Any())
        {
            return;
        }

        using var session = await _mongoDatabase.Client.StartSessionAsync();
        var transactionOptions = new TransactionOptions(ReadConcern.Snapshot, ReadPreference.Primary, WriteConcern.WMajority);
        session.StartTransaction(transactionOptions);
        try
        {
            var bulkOps = new List<WriteModel<EventData>>();
            foreach (var domainEvent in domainEvents)
            {
                var eventData = new EventData(domainEvent, await GetNextSequence(domainEvent.Id));
                ////var eventData = new EventData
                ////{
                ////    Id = Guid.NewGuid(),
                ////    StreamId = @event.Id, // TODO: ftechmax: Add StreamId indexing
                ////    TimeStamp = @event.TimeStamp,
                ////    Version = await GetNextSequence(@event.Id),
                ////    AssemblyQualifiedName = @event.GetType().AssemblyQualifiedName,
                ////    PayLoad = @event
                ////};
                bulkOps.Add(new InsertOneModel<EventData>(eventData));
            }
            await _collection.BulkWriteAsync(session, bulkOps).ConfigureAwait(false);

            await session.CommitTransactionAsync().ConfigureAwait(false);
        }
        catch
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    private IMongoCollection<T> GetCollection<T>() where T : EventData
    {
        return _mongoDatabase.GetCollection<T>($"{nameof(EventData)}");
    }

    /// <summary>
    /// Get the next sequence number for the transaction
    /// </summary>
    /// <remarks>See: https://www.mongodb.com/docs/v2.2/tutorial/create-an-auto-incrementing-field/</remarks>
    /// <param name="streamId">The aggregate id</param>
    /// <returns>The next sequence number</returns>
    private async Task<int> GetNextSequence(Guid streamId)
    {
        var filter = Builders<EventData>.Filter.And(Builders<EventData>.Filter.Eq(EventData.StreamIdFieldName, streamId));
        var updates = Builders<EventData>.Update.Inc(EventData.VersionFieldName, 1);

        var result = await _collection.FindOneAndUpdateAsync(filter, updates);

        return result?.Version ?? 0;
    }
}