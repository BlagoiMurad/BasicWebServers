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

        public void Add(string name , string value)
        {
            var header = new Header(name, value);
            headers.Add(header.Name, header);

        }

        public IEnumerator<Header> GetEnumerator() => headers.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
