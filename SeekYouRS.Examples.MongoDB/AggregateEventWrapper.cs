using MongoDB.Bson.Serialization.Attributes;
using System;
using SeekYouRS.Examples.Events;

namespace SeekYouRS.Examples.MongoDB {
    public sealed class AggregateEventWrapper {
        [BsonId]
        public Guid Id { get; set; }

        public Guid AggregateReference { get; set; }

        public BaseEvent Event { get; set; }

    }
}