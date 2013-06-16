using SeekYouRS.Handler;

namespace SeekYouRS.Examples {
	public sealed class UserContext : Context {
		public UserContext(IExecuteCommands commandHandler, ReadModelHandler readModelHandler)
			: base(commandHandler, readModelHandler) {

		}
	}
}
