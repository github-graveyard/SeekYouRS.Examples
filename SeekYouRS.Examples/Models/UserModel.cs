using System;
using System.Collections.Generic;

namespace SeekYouRS.Examples.Models {
	public sealed class UserModel {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string EMail { get; set; }
		public string Password { get; set; }
	}
}
