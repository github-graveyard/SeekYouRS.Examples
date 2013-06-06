using MongoDB.Bson.Serialization.Attributes;

namespace SeekYouRS.Examples.Events {
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(UserCreated))]
    [BsonKnownTypes(typeof(UserDeleted))]
    [BsonKnownTypes(typeof(UserChanged))]
    [BsonKnownTypes(typeof(PictureAdded))]
    [BsonKnownTypes(typeof(PictureDeleted))]
    public abstract class BaseEvent {
    }
}
