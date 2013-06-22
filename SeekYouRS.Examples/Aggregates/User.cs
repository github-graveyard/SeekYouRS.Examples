using System;
using SeekYouRS.Examples.Events;

namespace SeekYouRS.Examples.Aggregates {
    public sealed class User : Aggregate {
        public override Guid Id {
            get {
                return (FromHistory<UserDeleted>() != null)
                    ? Guid.Empty
                    : FromHistory<UserCreated>().Id;
                    
            }
        }

        public string Name{
            get {
                if (FromHistory<UserDeleted>() != null)
                    return null;

                var lastChange = FromHistory<UserChanged>();
                return lastChange != null
                    ? lastChange.Name
                    : FromHistory<UserCreated>().Name;
            }
        }

        public string EMail {
            get {
                if (FromHistory<UserDeleted>() != null)
                    return null;

                var lastChange = FromHistory<UserChanged>();
                return (lastChange != null)
                    ? lastChange.EMail
                    : FromHistory<UserCreated>().EMail;
            }
        }

        public string Password {
            get {
                if (FromHistory<UserDeleted>() != null)
                    return null;

                var lastChange = FromHistory<UserChanged>();
                return lastChange != null
                    ? lastChange.Password
                    : FromHistory<UserCreated>().Password;
            }
        }

        public void Create(Guid id, string name, string email, string password) {
            ApplyChanges(new UserCreated { Id = id, Name = name, EMail = email, Password = password });
        }

        public void Change(string name, string email, string password) {
            ApplyChanges(new UserChanged { Name = name, EMail = email, Password = password});
        }

        public void Delete() {
            ApplyChanges(new UserDeleted { Id = Id });
        }

    }
}