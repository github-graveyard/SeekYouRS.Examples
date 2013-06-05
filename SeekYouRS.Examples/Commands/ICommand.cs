using System;

namespace SeekYouRS.Examples.Commands {
    public interface ICommand {
        Guid Id { get; set; }
    }
}
