using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeekYouRS.Examples.Models;
using SeekYouRS.Store;

namespace SeekYouRS.Examples.Queries {
	public sealed class UserQueries : Store.Queries {
		public UserQueries(IStoreReadModels readModelStore) : base(readModelStore) {
		}

		public override T Retrieve<T>(dynamic query) {
			return ExecuteQuery(query);
		}

		private UserModel ExecuteQuery(GetUser query) {
			return ReadModelStore.Query<UserModel>().SingleOrDefault(u => u.Id == query.Id);
		}
		private IEnumerable<UserModel> ExecuteQuery(GetAllUsers query) {
			return ReadModelStore.Query<UserModel>().ToList();
		}

	}
}
