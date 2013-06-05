using SeekYouRS.Messaging;
using SeekYouRS.Storing;

namespace SeekYouRS.Examples.Handler {
    public sealed class UserQueries : IRetrieveModels {
        private readonly IStoreAggregateEventsAsModels _ReadModel;

        public UserQueries(IStoreAggregateEventsAsModels readModel) {
            _ReadModel = readModel;
        }

        public T Execute<T>(dynamic query) {
            return _ReadModel.Retrieve<T>(query);
        }

        public IStoreAggregateEventsAsModels ModelStore { get { return _ReadModel; } }
    }
}
