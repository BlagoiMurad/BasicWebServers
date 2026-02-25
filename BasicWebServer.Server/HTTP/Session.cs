using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.HTTP
{
    public class Session
    {

        public const string SessionCookieName = "SessionId";
        public const string CurrentDateKey = "CurrentDate";
        public const string UserKey = "User";

        private readonly Dictionary<string, string> data
            = new Dictionary<string, string>();

        public Session(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public string this[string key]
        {
            get => data[key];
            set => data[key] = value;
        }

        public bool ContainsKey(string key)
            => data.ContainsKey(key);

        public void Clear()
            => data.Clear();
    }
}
