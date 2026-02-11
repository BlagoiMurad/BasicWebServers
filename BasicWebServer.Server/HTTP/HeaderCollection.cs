using System.Collections.Generic;

namespace BasicWebServer.Server.HTTP
{
    public class HeaderCollection
    {
        private readonly Dictionary<string, Header> headers;

        public HeaderCollection()
        {
            this.headers = new Dictionary<string, Header>();
        }

        public int Count => this.headers.Count;

        public void Add(Header header)
        {
            this.headers[header.Name] = header;
        }
    }
}
