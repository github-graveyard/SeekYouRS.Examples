using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CQRSTest.Models {
    public sealed class UserModel {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
    }
}
