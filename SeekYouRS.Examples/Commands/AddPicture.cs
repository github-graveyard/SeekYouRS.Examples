using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeekYouRS.Examples.Models;

namespace SeekYouRS.Examples.Commands {
    public sealed class AddPicture : ICommand {
        public Guid Id { get; set; }
        public PictureModel Picture { get; set; }
    }
}
