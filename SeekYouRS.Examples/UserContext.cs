using SeekYouRS.Handler;
using SeekYouRS.Store;

namespace SeekYouRS.Examples {
	public sealed class UserContext : Context {
		public UserContext(IExecuteCommands commands, IQueryReadModels queries, IHandleAggregateEvents eventHandler) : base(commands, queries, eventHandler) {
		}
	}
}
