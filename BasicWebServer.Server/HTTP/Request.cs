using System;
using System.Linq;
using System.Web;

namespace BasicWebServer.Server.HTTP
{
    public class Request
    {
       

        public Request()
        {
        }

        public Method Method { get; }

        public string Url { get; }

        public HeaderCollection Headers { get; }

        public string Body { get; }

        public IReadOnlyDictionary<string, string> Form { get; private set; }
     
        public Request(Method method, string url, HeaderCollection headers, string body, Dictionary<string, string> form)
        {
            this.Method = method;
            this.Url = url;
            this.Headers = headers;
            this.Body = body;
            this.Form = form;
        }

     

     

      

        public static Request Parse(string request)
        {
            string[] lines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);

            string[] startLine = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string methodString = startLine[0];
            string url = startLine[1];

            Method method = ParseMethod(methodString);

            string[] headerLines = lines
                .Skip(1)
                .TakeWhile(x => x != string.Empty)
                .ToArray();

            HeaderCollection headers = ParseHeaders(headerLines);

            string[] bodyLines = lines.Skip(1 + headerLines.Length + 1).ToArray();
            string body = string.Join("\r\n", bodyLines);

            var form = ParseForm(headers, body);

            return new Request(method, url, headers, body, form);
        }


        private static Dictionary<string, string> ParseForm(HeaderCollection headers, string body)
        {
            var formCollection = new Dictionary<string, string>();
            if (headers.Contains(Header.ContentType) && headers[Header.ContentType] == ContentType.UrlEncoded)
            {
                var parsedResult = ParseFormData(body);

                foreach (var (key, value) in parsedResult)
                {
                    formCollection.Add(key, value);
                }
            }

            return formCollection;

        }

        private static Dictionary<string, string> ParseFormData(string bodyLines)
        {
            return HttpUtility.HtmlDecode(bodyLines)
                .Split('&')
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0], x => x[1]);


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
