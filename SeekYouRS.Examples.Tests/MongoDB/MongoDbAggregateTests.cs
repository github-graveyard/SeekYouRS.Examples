using System;
using System.Linq;
using FluentAssertions;
using MongoDB.Driver;
using NUnit.Framework;
using SeekYouRS.Examples.Aggregates;
using SeekYouRS.Examples.Events;
using SeekYouRS.Examples.Models;
using SeekYouRS.Examples.MongoDB;

namespace SeekYouRS.Examples.Tests.MongoDB {
	[TestFixture]
	public class MongoDbAggregateTests {

		private const string MongoConnectionString = "mongodb://cqrs-user:letmein@localhost:27017/cqrs-test";
		private const string TestDbName = "cqrs-test";

		[SetUp]
		public void Setup() {
			// Remove existing dbs and collections to make sure the test environment is always the same

			var client = new MongoClient(MongoConnectionString);
			var server = client.GetServer();
			server.Connect();

			if (server.GetDatabaseNames().Any(n => n == TestDbName)) {
				var db = server.GetDatabase(TestDbName);
				if (db.CollectionExists(MongoDbAggregateStore.AggregateEventCollectionName))
					db.DropCollection(MongoDbAggregateStore.AggregateEventCollectionName);

				if (db.CollectionExists(MongoDbUserReadModelStore.GetCollectionNameByType<UserModel>()))
					db.DropCollection(MongoDbUserReadModelStore.GetCollectionNameByType<UserModel>());
			}

			server.Disconnect();
		}

		[Test]
		public void TestToCreateUserViaAggregate() {
			var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);
			var aggregates = new Store.Aggregates(aggregateStore);

			var uid = Guid.NewGuid();
			var user = aggregates.GetAggregate<User>(uid);
			user.Create(uid, "Rüdiger", "rüdiger@mail.com", "swordfish");
			aggregates.Save(user);
			user.Name.ShouldAllBeEquivalentTo("Rüdiger");
			user.EMail.ShouldBeEquivalentTo("rüdiger@mail.com");
			user.Password.ShouldBeEquivalentTo("swordfish");
		}

		[Test]
		public void TestToCreateUser() {
			var userId = Guid.NewGuid();
			var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);
			var aggregates = new Store.Aggregates(aggregateStore);

			var user = aggregates.GetAggregate<User>(userId);
			user.Create(userId, "Rüdiger", "rüdiger@mail.com", "swordfish");
			aggregates.Save(user);

			user.Name.Should().BeEquivalentTo("Rüdiger");
			user.EMail.Should().BeEquivalentTo("rüdiger@mail.com");
			user.Password.Should().BeEquivalentTo("swordfish");
			aggregates.GetAggregate<User>(userId)
			          .History.OfType<AggregateEventBag<UserCreated>>().Any()
			          .Should().BeTrue();
		}

		[Test]
		public void TestToChangeUsername() {
			var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);
			var userId = Guid.NewGuid();
			var aggregates = new Store.Aggregates(aggregateStore);
			var user = aggregates.GetAggregate<User>(userId);

			user.Create(userId, "Rüdiger", "rüdiger@mail.com", "swordfish");
			aggregates.Save(user);

			user.Name.Should().BeEquivalentTo("Rüdiger");
			user.EMail.Should().BeEquivalentTo("rüdiger@mail.com");

			user.Change("Rüdiger Horstmann", "horst@mymail.com", "abc");
			user.Name.Should().BeEquivalentTo("Rüdiger Horstmann");
			user.EMail.Should().BeEquivalentTo("horst@mymail.com");
			aggregates.Save(user);

			user.Changes.Count.ShouldBeEquivalentTo(0);
			user.History.Count().ShouldBeEquivalentTo(2);
		}
	}
}