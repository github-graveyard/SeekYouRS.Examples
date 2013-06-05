using System;
using SeekYouRS.Examples.Aggregates;
using SeekYouRS.Examples.Commands;
using SeekYouRS.Messaging;
using SeekYouRS.Storing;

namespace SeekYouRS.Examples.Handler {
    public class UserCommands : IExecuteCommands {
        public event Action<AggregateEvent> Performed;

        private readonly IStoreAggregates _AggregateEventStore;

        public UserCommands(IStoreAggregates aggregateEventStore) {
            _AggregateEventStore = aggregateEventStore;
            _AggregateEventStore.AggregateHasChanged += OnEventPublished;
        }

        private void OnEventPublished(AggregateEvent aggregateEvent) {
            var handler = Performed;
            if (handler != null)
                handler(aggregateEvent);
        }

        public void Process(dynamic command) {
            Execute(command);
        }

        private void Execute(object command) {
            throw new ArgumentException(string.Format("Command not implemented: {0}", command.GetType().Name));
        }

        private void Execute(CreateUser command) {
            var user = _AggregateEventStore.GetAggregate<User>(command.Id);
            user.Create(command.Id, command.Name, command.EMail, command.Password);
            _AggregateEventStore.Save(user);
        }

        private void Execute(ChangeUser command) {
            var user = _AggregateEventStore.GetAggregate<User>(command.Id);
            user.Change(command.Name, command.EMail, command.Password);
            _AggregateEventStore.Save(user);
        }

        private void Execute(DeleteUser command) {
            var user = _AggregateEventStore.GetAggregate<User>(command.Id);
            user.Delete();
            _AggregateEventStore.Save(user);
        }

    }
}