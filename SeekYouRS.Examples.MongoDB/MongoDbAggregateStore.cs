using SeekYouRS.Examples.Events;
using SeekYouRS.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace SeekYouRS.Examples.MongoDB {

	public sealed class MongoDbAggregateStore : IStoreAndRetrieveAggregates, IDisposable {

		private readonly string _MongoConnectionString;
		private readonly MongoClient _MongoClient;
		private readonly MongoServer _MongoServer;
		private readonly MongoDatabase _MongoDatabase;
		private readonly MongoCollection<AggregateEventWrapper> _Collection;

		private const string DbName = "cqrs-test";
		public const string AggregateEventCollectionName = "aggregateevents";

		public MongoDbAggregateStore(string mongoConnectionString) {
			_MongoConnectionString = mongoConnectionString;
			_MongoClient = new MongoClient(mongoConnectionString);
			_MongoServer = _MongoClient.GetServer();
			_MongoServer.Connect();
			_MongoDatabase = _MongoServer.GetDatabase(DbName);
			_Collection = _MongoDatabase.GetCollection<AggregateEventWrapper>(AggregateEventCollectionName);
		}

		public void Save(IEnumerable<AggregateEvent> changes) {
			changes
				.ToList()
				.ForEach(c => _Collection.Insert(new AggregateEventWrapper {
					                                                           Id = Guid.NewGuid(),
					                                                           AggregateReference = c.Id,
					                                                           TimeStamp = (DateTime) c.GetType().GetProperty("Timestamp").GetValue(c),
					                                                           Event = c.GetType().GetProperty("EventData").GetValue(c) as BaseEvent
				                                                           }));
		}

		public IEnumerable<AggregateEvent> GetEventsBy(Guid id) {
			var storedEvents = _Collection.FindAs<AggregateEventWrapper>(Query.EQ("AggregateReference", id));
			var events = new List<AggregateEvent>();

			foreach (var storedEvent in storedEvents) {
				var eventBagType = typeof (AggregateEventBag<>).MakeGenericType(storedEvent.Event.GetType());
				var eventBagInstance = Activator.CreateInstance(eventBagType, new object[] {storedEvent.AggregateReference, storedEvent.TimeStamp});
				eventBagInstance.GetType().GetProperty("EventData").SetValue(eventBagInstance, storedEvent.Event);
				events.Add((AggregateEvent) eventBagInstance);
			}

			return events;
		}

		public void Dispose() {
			if(_MongoServer.State == MongoServerState.Connected)
				_MongoServer.Disconnect();
		}
	}
}
