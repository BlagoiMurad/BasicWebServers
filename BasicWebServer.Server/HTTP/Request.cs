using System;
using System.Linq;

namespace BasicWebServer.Server.HTTP
{
    public class Request
    {
        public Request(Method method, string url, HeaderCollection headers, string body)
        {
            this.Method = method;
            this.Url = url;
            this.Headers = headers;
            this.Body = body;
        }

        public Method Method { get; }

        public string Url { get; }

        public HeaderCollection Headers { get; }

        public string Body { get; }

        public static Request Parse(string request)
        {
            string[] lines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);

            string[] startLine = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string methodString = startLine[0];
            string url = startLine[1];

            Method method = ParseMethod(methodString);

            string[] headerLines = lines.Skip(1).TakeWhile(x => x != string.Empty).ToArray();
            HeaderCollection headers = ParseHeaders(headerLines);

            string[] bodyLines = lines.Skip(1 + headerLines.Length + 1).ToArray();
            string body = string.Join("\r\n", bodyLines);

            return new Request(method, url, headers, body);
        }

        private static Method ParseMethod(string method)
        {
            bool parsed = Enum.TryParse(method, true, out Method parsedMethod);

            if (!parsed)
            {
                throw new InvalidOperationException("Invalid request method.");
            }

            return parsedMethod;
        }

        private static HeaderCollection ParseHeaders(string[] headersLines)
        {
            HeaderCollection headers = new HeaderCollection();

            foreach (var headerLine in headersLines)
            {
                string[] headerParts = headerLine.Split(": ", 2, StringSplitOptions.RemoveEmptyEntries);

                if (headerParts.Length != 2)
                {
                    throw new InvalidOperationException("Invalid request header.");
                }

                string headerName = headerParts[0];
                string headerValue = headerParts[1];

                Header header = new Header(headerName, headerValue);

                headers.Add(header);
            }

            return headers;
        }
    }
}
