using System;

namespace SeekYouRS.Examples.Commands {
    public class CreateUser : ICommand {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
    }
}
