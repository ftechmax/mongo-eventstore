namespace MongoEventStore;

internal class EventStoreConfig
{
    public EventStoreConfig(string databaseName)
    {
        DatabaseName = databaseName;
    }

    public string DatabaseName { get; }
}