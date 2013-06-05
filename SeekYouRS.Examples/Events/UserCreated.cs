using System;

namespace SeekYouRS.Examples.Events {
    public class UserCreated : BaseEvent {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
    }
}
