namespace SeekYouRS.Examples.Events {
    public class UserChanged : BaseEvent {
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
    }
}
