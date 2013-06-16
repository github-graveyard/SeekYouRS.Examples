using SeekYouRS.Examples.Events;
using SeekYouRS.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace SeekYouRS.Examples.MongoDB {

	public sealed class MongoDbAggregateStore : IStoreAggregates {

		private readonly string _MongoConnectionString;
		private const string DbName = "cqrs-test";
		public const string AggregateEventCollectionName = "aggregateevents";

		public event Action<AggregateEvent> AggregateHasChanged;

		public MongoDbAggregateStore(string mongoConnectionString) {
			_MongoConnectionString = mongoConnectionString;
		}

		private void OnAggregateHasChanged(AggregateEvent obj) {
			var handler = AggregateHasChanged;
			if (handler != null) handler(obj);
		}

		public TAggregate GetAggregate<TAggregate>(Guid aggregateId) where TAggregate : Aggregate, new() {
			var mConnection = new MongoClient(_MongoConnectionString);
			var mServer = mConnection.GetServer();
			mServer.Connect();
			var db = mServer.GetDatabase(DbName);
			var collection = db.GetCollection<AggregateEventWrapper>(AggregateEventCollectionName);

			var aggregateHistory = collection.FindAs<AggregateEventWrapper>(Query.EQ("AggregateReference", aggregateId)).ToList();

			var aggregate = new TAggregate {
				History = new List<AggregateEvent>()
			};

			foreach (var historyEntry in aggregateHistory) {
				var eventType = historyEntry.Event.GetType();
				var eventBagType = typeof (AggregateEventBag<>).MakeGenericType(eventType);
				var eInstance = Activator.CreateInstance(eventBagType, new object[] {historyEntry.AggregateReference, historyEntry.TimeStamp});
				eInstance.GetType().GetProperty("EventData").SetValue(eInstance, historyEntry.Event);
				((List<AggregateEvent>)aggregate.History).Add(eInstance as AggregateEvent);
			}

			mServer.Disconnect();
			return aggregate;
		}

		public void Save<TAggregate>(TAggregate aggregate) where TAggregate : SeekYouRS.Aggregate {
			var mConnection = new MongoClient(_MongoConnectionString);
			var mServer = mConnection.GetServer();
			mServer.Connect();
			var db = mServer.GetDatabase(DbName);
			var collection = db.GetCollection<AggregateEventWrapper>(AggregateEventCollectionName);
			aggregate.Changes
					 .ToList()
					 .ForEach(c => {
								  var wrapper = new AggregateEventWrapper {
																			  Id = Guid.NewGuid(),
																			  AggregateReference = aggregate.Id,
																			  TimeStamp = (DateTime)c.GetType().GetProperty("Timestamp").GetValue(c),
																			  Event = c.GetType().GetProperty("EventData").GetValue(c) as BaseEvent
																		  };
								  collection.Insert(wrapper);
								  OnAggregateHasChanged(c);
							  });
			
			aggregate.History = aggregate.History.Concat(aggregate.Changes).ToList();
			aggregate.Changes.Clear();

			mServer.Disconnect();
		}
  
	}
}
