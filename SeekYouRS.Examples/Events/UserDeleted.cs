using System;

namespace SeekYouRS.Examples.Events {
    public class UserDeleted : BaseEvent {
        public Guid Id { get; set; }
    }
}
