using System.Collections;
using System.Collections.Generic;

namespace BasicWebServer.Server.HTTP
{
    public class HeaderCollection : IEnumerable<Header>
    {

        private readonly Dictionary<string, Header> headers;

        public HeaderCollection()
        {
            this.headers = new Dictionary<string, Header>();
        }

        public int Count => this.headers.Count;

        public void Add(string name, string value)
        {
            var header = new Header(name, value);
            this.headers[name] = header;
        }

        public bool Contains(string name)
        {
            return headers.ContainsKey(name);
        }

        public string this[string name]
        {
            get
            {
                return headers[name].Value;
            }

            set
            {
                headers[name].Value = value;
            }
        }
        public IEnumerator<Header> GetEnumerator()
        {
            return headers.Values.GetEnumerator();
        }

        internal void Add(Header header)
        {
            this.headers[header.Name] = header;
        }



        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}


