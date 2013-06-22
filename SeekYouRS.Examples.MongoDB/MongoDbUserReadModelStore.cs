using System.Collections.Generic;
using MongoDB.Driver;
using SeekYouRS.Examples.Models;
using SeekYouRS.Store;
using System;
using System.Linq;
using MDB=MongoDB.Driver.Builders;

namespace SeekYouRS.Examples.MongoDB {
	public sealed class MongoDbUserReadModelStore : IStoreReadModels {
		private readonly MongoClient _MongoClient;
		private readonly MongoServer _MongoServer;
		private readonly MongoDatabase _MongoDatabase;

		private const string DbName = "cqrs-test";
		public const string UsersCollectionName = "users";

		public MongoDbUserReadModelStore(string mongoDbConnectionString) {
			_MongoClient = new MongoClient(mongoDbConnectionString);
			_MongoServer = _MongoClient.GetServer();
			_MongoServer.Connect();
			_MongoDatabase = _MongoServer.GetDatabase(DbName);
		}

		public void Add(dynamic model) {
			PerformAdd(model);
		}

		private void PerformAdd(object model) {
			throw new Exception(string.Format("Invalid model: {0}", model.GetType().Name));
		}

		private void PerformAdd(UserModel model) {
			var collection = _MongoDatabase.GetCollection<UserModel>(GetCollectionNameByType<UserModel>());
			collection.Insert(model);
		}

		public void Change(dynamic model) {
			PerformChange(model);
		}

		private void PerformChange(object model) {
			throw new Exception(string.Format("Invalid model: {0}", model.GetType().Name));			
		}

		private void PerformChange(UserModel model) {
			var collection = _MongoDatabase.GetCollection<UserModel>(GetCollectionNameByType<UserModel>());
			var user = collection.FindOneAs<UserModel>(MDB.Query.EQ("_id", model.Id));
			user.EMail = model.EMail;
			user.Name = model.Name;
			user.Password = model.Password;
			collection.Save(user);
		}

		public void Remove(dynamic model) {
			PerformRemove(model);
		}

		private void PerformRemove(object model) {
			throw new Exception(string.Format("Invalid model: {0}", model.GetType().Name));			
		}

		private void PerformRemove(UserModel model) {
			var collection = _MongoDatabase.GetCollection<UserModel>(GetCollectionNameByType<UserModel>());
			collection.Remove(MDB.Query.EQ("_id", model.Id));
		}

		public IEnumerable<T> Query<T>() {
			return _MongoDatabase.GetCollection<T>(GetCollectionNameByType<T>())
				.FindAll()
				.AsQueryable();
		}

		public static string GetCollectionNameByType<T>() {
			return typeof (T).Name.ToLowerInvariant();
		}

	}
}