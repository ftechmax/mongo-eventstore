using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoEventStore;

internal class EventData
{
    public const string IdFieldName = "_id";

    public const string StreamIdFieldName = "_streamId";

    public const string VersionFieldName = "_version";

    public EventData(IDomainEvent @event, int version)
    {
        Id = Guid.NewGuid();
        StreamId = @event.Id;
        Version = version;
        PayLoad = @event;
        TimeStamp = @event.TimeStamp;
        AssemblyQualifiedName = @event.GetType().AssemblyQualifiedName!;
    }

    [BsonElement(IdFieldName)]
    [BsonId(IdGenerator = typeof(GuidGenerator))]
    public Guid Id { get; private set; }

    [BsonElement(StreamIdFieldName)]
    public Guid StreamId { get; private set; }

    [BsonElement(VersionFieldName)]
    public int Version { get; private set; }

    [BsonElement("_payload")]
    [BsonSerializer(typeof(ImpliedImplementationInterfaceSerializer<IDomainEvent, DomainEvent>))]
    public IDomainEvent PayLoad { get; private set; }

    [BsonElement("_timestamp")]
    public DateTimeOffset TimeStamp { get; private set; }

    [BsonElement("_clrTypeFullname")]
    public string AssemblyQualifiedName { get; private set; }
}