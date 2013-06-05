using System;

namespace SeekYouRS.Examples.Events {
    public class PictureDeleted : BaseEvent {
        public Guid Id { get; set; }
        public Guid PictureId { get; set; }
    }
}
