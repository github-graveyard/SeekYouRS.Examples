using System;
using System.Linq;
using FluentAssertions;
using MongoDB.Driver;
using NUnit.Framework;
using SeekYouRS.Examples.Aggregates;
using SeekYouRS.Examples.Events;
using SeekYouRS.Examples.Models;
using SeekYouRS.Examples.MongoDB;
using SeekYouRS.Storing;

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
                
                if (db.CollectionExists(MongoDbReadModel.UsersCollectionName))
                    db.DropCollection(MongoDbReadModel.UsersCollectionName);
            }

            server.Disconnect();
        }

        [Test]
        public void TestToCreateUserViaAggregate() {
            var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);
            var id = Guid.NewGuid();
            var user = aggregateStore.GetAggregate<User>(id);

            user.Create(id, "My User", "test@test.com", "124");
            aggregateStore.Save(user);

            user.Name.Should().BeEquivalentTo("My User");
            user.EMail.Should().BeEquivalentTo("test@test.com");
            user.Password.Should().BeEquivalentTo("124");
        }

        [Test]
        public void TestToCreateUser() {
            var userId = Guid.NewGuid();
            var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);

            var user = aggregateStore.GetAggregate<User>(userId);
            user.Create(userId, "My User","user@user.com", "123");
            aggregateStore.Save(user);

            user.Name.Should().BeEquivalentTo("My User");
            user.EMail.Should().BeEquivalentTo("user@user.com");
            user.Password.Should().BeEquivalentTo("123");
            aggregateStore.GetAggregate<User>(userId)
                          .History.OfType<AggregateEventBag<UserCreated>>().Any()
                          .Should().BeTrue();
        }

        [Test]
        public void TestToChangeUsername() {
            var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);
            var userId = Guid.NewGuid();
            var user = aggregateStore.GetAggregate<User>(userId);

            user.Create(userId, "Horst", "test@test.com", "123");
            aggregateStore.Save(user);

            user.Name.Should().BeEquivalentTo("Horst");
            user.EMail.Should().BeEquivalentTo("test@test.com");

            user.Change("Horst Horstmann", "horst@mymail.com", "abc");
            user.Name.Should().BeEquivalentTo("Horst Horstmann");
            user.EMail.Should().BeEquivalentTo("horst@mymail.com");
            aggregateStore.Save(user);
            
            user.Changes.Count.ShouldBeEquivalentTo(0);
            user.History.Count().ShouldBeEquivalentTo(2);
        }

        [Test]
        public void TestToAddAndRemovePictures() {
            var aggregateStore = new MongoDbAggregateStore(MongoConnectionString);
            var userId = Guid.NewGuid();
            var user = aggregateStore.GetAggregate<User>(userId);

            user.Create(userId, "Horst", "test@test.com", "123");
            aggregateStore.Save(user);

            user.Name.Should().BeEquivalentTo("Horst");
            user.EMail.Should().BeEquivalentTo("test@test.com");
            user.Pictures.Count.ShouldBeEquivalentTo(0);

            var pic = new PictureModel {
                                           Id = Guid.NewGuid(),
                                           Name = "MyPic.jpg",
                                           Url = "http://cdn.somewhere.com/123/yay.jpg"
                                       };
            user.AddPicture(pic);
            user.Pictures.Count.ShouldBeEquivalentTo(1);

            user.DeletePicture(pic.Id);
            user.Pictures.Count.ShouldBeEquivalentTo(0);
            aggregateStore.Save(user);
        }
    }
}
