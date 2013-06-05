using System;

namespace SeekYouRS.Examples.Commands {
    public class DeleteUser : ICommand {
        public Guid Id { get; set; }
    }
}
