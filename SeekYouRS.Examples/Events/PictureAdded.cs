using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeekYouRS.Examples.Models;

namespace SeekYouRS.Examples.Events {
    public sealed class PictureAdded : BaseEvent {
        public PictureModel Picture { get; set; }
    }
}
