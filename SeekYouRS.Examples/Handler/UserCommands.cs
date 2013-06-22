using SeekYouRS.Examples.Aggregates;
using SeekYouRS.Examples.Commands;
using SeekYouRS.Handler;


namespace SeekYouRS.Examples.Handler {
	public class UserCommands : CommandHandler {
		
		public UserCommands(Store.Aggregates aggregatesRepository) : base(aggregatesRepository) {
		}

		public override void Process(dynamic command) {
			Execute(command);
		}

		private void Execute(CreateUser command) {
			var user = AggregateStore.GetAggregate<User>(command.Id);
			user.Create(command.Id, command.Name, command.EMail, command.Password);
			AggregateStore.Save(user);
		}

		private void Execute(ChangeUser command) {
			var user = AggregateStore.GetAggregate<User>(command.Id);
			user.Change(command.Name, command.EMail, command.Password);
			AggregateStore.Save(user);
		}

		private void Execute(DeleteUser command) {
			var user = AggregateStore.GetAggregate<User>(command.Id);
			user.Delete();
			AggregateStore.Save(user);
		}

	}
}