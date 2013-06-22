using MongoDB.Bson.Serialization.Attributes;

namespace SeekYouRS.Examples.Events {
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(UserCreated))]
    [BsonKnownTypes(typeof(UserDeleted))]
    [BsonKnownTypes(typeof(UserChanged))]
    public abstract class BaseEvent {
    }
}
