using System;

namespace SeekYouRS.Examples.Commands {
    public sealed class DeletePicture : ICommand {
        public Guid Id { get; set; }
        public Guid PictureId { get; set; }
    }
}
