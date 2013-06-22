using SeekYouRS.Examples.Events;
using SeekYouRS.Examples.Models;
using SeekYouRS.Handler;
using SeekYouRS.Store;

namespace SeekYouRS.Examples.Handler {
	public sealed class UserReadModelHandler : ReadModelHandler {
		public UserReadModelHandler(IStoreReadModels readModelStore) : base(readModelStore) {
		}

		public override void SaveChangesBy(AggregateEvent aggregateEvent) {
			Handle((dynamic)aggregateEvent);
		}


		private void Handle(AggregateEventBag<UserCreated> userCreated) {
			ReadModelStore.Add(new UserModel {
				                                 Id = userCreated.EventData.Id,
				                                 EMail = userCreated.EventData.EMail,
				                                 Name = userCreated.EventData.Name,
				                                 Password = userCreated.EventData.Password
			                                 });
		}

		private void Handle(AggregateEventBag<UserChanged> userChanged) {
			ReadModelStore.Change(new UserModel {
				                                    Id = userChanged.Id,
				                                    EMail = userChanged.EventData.EMail,
				                                    Name = userChanged.EventData.Name,
				                                    Password = userChanged.EventData.Password
			                                    });
		}

		private void Handle(AggregateEventBag<UserDeleted> userDeleted) {
			ReadModelStore.Remove(new UserModel {Id = userDeleted.Id});
		}

	}
}