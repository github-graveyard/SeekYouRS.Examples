using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using SeekYouRS.Examples.Events;
using SeekYouRS.Examples.Models;
using SeekYouRS.Examples.Queries;
using SeekYouRS.Storing;
using System;
using System.Linq;

namespace SeekYouRS.Examples.MongoDB {
    public sealed class MongoDbReadModel : IStoreAggregateEventsAsModels{
        private readonly string _MongoDbConnectionString;
        private const string DbName = "cqrs-test";
        public const string UsersCollectionName = "users";

        public MongoDbReadModel(string mongoDbConnectionString) {
            _MongoDbConnectionString = mongoDbConnectionString;
        }

        public void HandleChanges(AggregateEvent aggregateEvent) {
            Handle((dynamic)aggregateEvent);
        }

        private void Handle(object unassignedEvent) {
            var eventData = ((dynamic)unassignedEvent).EventData;
            throw new ArgumentException("This event is not assigned to this instance, " + eventData.GetType().Name);
        }

        private void Handle(AggregateEventBag<UserCreated> userCreated) {
            Add(new UserModel {
                                  Id = userCreated.Id,
                                  Name = userCreated.EventData.Name,
                                  EMail = userCreated.EventData.EMail,
                                  Password = userCreated.EventData.Password
                              });
        }

        private void Handle(AggregateEventBag<UserChanged> userChanged) {
            Change(new UserModel {
                                     Id = userChanged.Id,
                                     Name = userChanged.EventData.Name,
                                     EMail = userChanged.EventData.EMail,
                                     Password = userChanged.EventData.Password
                                 });
        }
        
        private void Handle(AggregateEventBag<UserDeleted> userDeleted) {
            Remove(new UserModel {
                                     Id = userDeleted.Id
                                 });
        }

        private void Handle(AggregateEventBag<PictureAdded> pictureAdded) {
            AddPicture(new UserModel {Id = pictureAdded.Id}, pictureAdded.EventData.Picture);
        }

        private void Handle(AggregateEventBag<PictureDeleted> pictureDeleted) {
            RemovePicture(new UserModel{Id = pictureDeleted.Id}, pictureDeleted.EventData.PictureId);
        }

        private void Add(UserModel user) {
            var s = CreateAndConnectMongoServer();
            var db = s.GetDatabase(DbName);
            var coll = db.GetCollection<UserModel>(UsersCollectionName);
            coll.Insert(user);
            s.Disconnect();
        }

        private void Change(UserModel model) {
            var s = CreateAndConnectMongoServer();
            var db = s.GetDatabase(DbName);
            var coll = db.GetCollection<UserModel>(UsersCollectionName);
            var u = coll.FindOneAs<UserModel>(Query.EQ("_id", model.Id));
            if (u != null) {
                u.EMail = model.EMail;
                u.Name = model.Name;
                u.Password = model.Password;
                coll.Save(u);
            }
            else
                Add(model);

            s.Disconnect();
        }

        private void AddPicture(UserModel model, PictureModel picture) {
            var s = CreateAndConnectMongoServer();
            var db = s.GetDatabase(DbName);
            var coll = db.GetCollection<UserModel>(UsersCollectionName);
            var user = coll.FindOneAs<UserModel>(Query.EQ("_id", model.Id));
            user.Pictures.Add(picture);
            coll.Save(user);
            s.Disconnect();
        }

        private void RemovePicture(UserModel model, Guid pictureId) {
            var s = CreateAndConnectMongoServer();
            var db = s.GetDatabase(DbName);
            var coll = db.GetCollection<UserModel>(UsersCollectionName);
            var user = coll.FindOneAs<UserModel>(Query.EQ("_id", model.Id));
            var p = user.Pictures.FirstOrDefault(pic => pic.Id == pictureId);
            if (p != null) {
                user.Pictures.Remove(p);
                coll.Save(user);
            }
            s.Disconnect();
        }

        private void Remove(UserModel model) {
            var s = CreateAndConnectMongoServer();
            var db = s.GetDatabase(DbName);
            var coll = db.GetCollection<UserModel>(UsersCollectionName);
            coll.Remove(Query.EQ("_id", model.Id));
            s.Disconnect();
        }

        public T Retrieve<T>(dynamic query) {
            return ExecuteQuery(query);
        }
        
        private object ExecuteQuery(object query) {
            throw new InvalidOperationException(string.Format("Unhandled query: {0}", query.GetType().Name));
        }
        private UserModel ExecuteQuery(GetUser query) {
            var s = CreateAndConnectMongoServer();
            var db = s.GetDatabase(DbName);
            var coll = db.GetCollection<UserModel>(UsersCollectionName);
            return coll.Find(Query.EQ("_id", query.Id)).FirstOrDefault();
        }
        private List<UserModel> ExecuteQuery(GetAllUsers query) {
            var s = CreateAndConnectMongoServer();
            var db = s.GetDatabase(DbName);
            var coll = db.GetCollection<UserModel>(UsersCollectionName);
            return coll.Find(Query.Null).ToList();
        }

        private MongoServer CreateAndConnectMongoServer() {
            var mClient = new MongoClient(_MongoDbConnectionString);
            var server = mClient.GetServer();
            server.Connect();
            return server;
        }
    }
}