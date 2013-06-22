using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Driver;
using NUnit.Framework;
using FluentAssertions;
using SeekYouRS.Examples.Commands;
using SeekYouRS.Examples.Handler;
using SeekYouRS.Examples.Models;
using SeekYouRS.Examples.MongoDB;
using SeekYouRS.Examples.Queries;
using SeekYouRS.Handler;

namespace SeekYouRS.Examples.Tests.MongoDB {
	[TestFixture]
	public sealed class MongoDbContextTests {
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
		public void TestToCreateUserAndReadIt() {
			var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);
			var aggregates = new Store.Aggregates(aggregateStore);
			var readModelStore = new MongoDbUserReadModelStore(MongoConnectionString);
			var readModelHandler = new UserReadModelHandler(readModelStore);

			var commands = new UserCommands(aggregates);
			var queries = new UserQueries(readModelStore);

			var context = new UserContext(commands, queries, readModelHandler);
			var uid = Guid.NewGuid();
			
			context.Process(new CreateUser{Id = uid, Name = "Rüdiger", EMail = "rüdiger@mail.com", Password = "swordfish"});
			var userModel = context.ExecuteQuery<UserModel>(new GetUser {Id = uid});
			userModel.Id.ShouldBeEquivalentTo(uid);
			userModel.Name.ShouldBeEquivalentTo("Rüdiger");
			userModel.EMail.ShouldBeEquivalentTo("rüdiger@mail.com");
			userModel.Password.ShouldBeEquivalentTo("swordfish");
		}

		[Test]
		public void TestToCreateAndChangeUserViaApi() {
			var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);
			var aggregates = new Store.Aggregates(aggregateStore);
			var readModelStore = new MongoDbUserReadModelStore(MongoConnectionString);
			var readModelHandler = new UserReadModelHandler(readModelStore);

			var commands = new UserCommands(aggregates);
			var queries = new UserQueries(readModelStore);

			var context = new UserContext(commands, queries, readModelHandler);
			var uid = Guid.NewGuid();

			context.Process(new CreateUser { Id = uid, Name = "Rüdiger", EMail = "rüdiger@mail.com", Password = "swordfish" });
			context.Process(new ChangeUser{Id = uid, Name = "Horst Horstmann", EMail = "horst@mail.com", Password = "abc"});

			var user = context.ExecuteQuery<UserModel>(new GetUser {Id = uid});
			user.Name.ShouldBeEquivalentTo("Horst Horstmann");
			user.EMail.ShouldBeEquivalentTo("horst@mail.com");
			user.Password.ShouldBeEquivalentTo("abc");
		}

		[Test]
		public void TestToCreateTwoUsersAndChangeOneOfThem() {
			var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);
			var aggregates = new Store.Aggregates(aggregateStore);
			var readModelStore = new MongoDbUserReadModelStore(MongoConnectionString);
			var readModelHandler = new UserReadModelHandler(readModelStore);

			var commands = new UserCommands(aggregates);
			var queries = new UserQueries(readModelStore);

			var context = new UserContext(commands, queries, readModelHandler);
			var uid1 = Guid.NewGuid();
			var uid2 = Guid.NewGuid();

			context.Process(new CreateUser {Id = uid1, Name = "Horst", EMail = "horst@mail.com", Password = "abc"});
			context.Process(new CreateUser {Id = uid2, Name = "Egon", EMail = "egon@mail.com", Password = "123"});
			context.Process(new ChangeUser {
										   Id = uid2,
										   Name = "Egon Müller",
										   EMail = "egon_mueller@mail.com",
										   Password = "123abc"
									   });

			var user = context.ExecuteQuery<UserModel>(new GetUser {Id = uid2});
			user.Name.ShouldBeEquivalentTo("Egon Müller");
			user.EMail.ShouldBeEquivalentTo("egon_mueller@mail.com");
			user.Password.ShouldBeEquivalentTo("123abc");
		}

		[Test]
		public void TestToGetAllUsers() {
			var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);
			var aggregates = new Store.Aggregates(aggregateStore);
			var readModelStore = new MongoDbUserReadModelStore(MongoConnectionString);
			var readModelHandler = new UserReadModelHandler(readModelStore);

			var commands = new UserCommands(aggregates);
			var queries = new UserQueries(readModelStore);

			var context = new UserContext(commands, queries, readModelHandler);

			for (var i = 0; i < 500; i++)
				context.Process(new CreateUser {
											   Id = Guid.NewGuid(),
											   Name = string.Format("User_{0}", i),
											   EMail = string.Format("user_{0}@mail.com", i),
											   Password = (i*123).ToString(CultureInfo.InvariantCulture)
										   });

			var users = context.ExecuteQuery<List<UserModel>>(new GetAllUsers());
			users.Count.ShouldBeEquivalentTo(500);
		}

		[Test]
		public void TestToDeleteUser() {
			var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);
			var aggregates = new Store.Aggregates(aggregateStore);
			var readModelStore = new MongoDbUserReadModelStore(MongoConnectionString);
			var readModelHandler = new UserReadModelHandler(readModelStore);

			var commands = new UserCommands(aggregates);
			var queries = new UserQueries(readModelStore);

			var context = new UserContext(commands, queries, readModelHandler);
			var id = Guid.NewGuid();

			context.Process(new CreateUser { Id = id, Name = "Testuser", EMail = "test@mail.com", Password = "123" });
			var user = context.ExecuteQuery<UserModel>(new GetUser { Id = id });
			user.Should().NotBeNull();

			context.Process(new DeleteUser{Id = id});
			user = context.ExecuteQuery<UserModel>(new GetUser {Id = id});
			user.Should().BeNull();
		}
	}
}
