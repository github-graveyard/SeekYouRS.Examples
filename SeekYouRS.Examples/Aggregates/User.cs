using System;
using System.Collections.Generic;
using System.Linq;
using SeekYouRS.Examples.Events;
using SeekYouRS.Examples.Models;
using SeekYouRS.Storing;

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

        public List<PictureModel> Pictures {
            get {
                if (FromHistory<UserDeleted>() != null)
                    return null;

                return (from h in History.Concat(Changes)
                        where h is AggregateEventBag<PictureAdded>
                              && h.Id == Id
                              && !History.Concat(Changes).Any(
                                  hDeleted =>
                                  hDeleted is AggregateEventBag<PictureDeleted> &&
                                  ((AggregateEventBag<PictureDeleted>) hDeleted).EventData.Id == Id &&
                                  ((AggregateEventBag<PictureDeleted>) hDeleted).EventData.PictureId == ((AggregateEventBag<PictureAdded>) h).EventData.Picture.Id)
                        select ((AggregateEventBag<PictureAdded>) h).EventData.Picture).ToList();
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

        public void AddPicture(PictureModel picture) {
            ApplyChanges(new PictureAdded {Picture = picture});
        }

        public void DeletePicture(Guid pictureId) {
            ApplyChanges(new PictureDeleted {Id = Id, PictureId = pictureId});
        }

    }
}