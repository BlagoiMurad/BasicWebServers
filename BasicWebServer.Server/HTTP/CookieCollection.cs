using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.HTTP
{
    public class CookieCollection : IEnumerable<Cookie>
    {
        private readonly Dictionary<string, Cookie> cookies
       = new Dictionary<string, Cookie>();

        public int Count => cookies.Count;

        public void Add(Cookie cookie)
            => cookies[cookie.Name] = cookie;

        public bool Contains(string name)
            => cookies.ContainsKey(name);

        public string this[string name]
        {
            get => cookies[name].Value;
            set => cookies[name] = new Cookie(name, value);
        }

        public IEnumerator<Cookie> GetEnumerator()
            => cookies.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}

