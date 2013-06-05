using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeekYouRS.Messaging;

namespace SeekYouRS.Examples {
    public sealed class UserApi : Api {
        public UserApi(IExecuteCommands commandHandler, IRetrieveModels queriesHandler) : base(commandHandler, queriesHandler) {

        }
    }
}
