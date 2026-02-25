using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasicWebServer.Server.HTTP
{
    public class Request
    {
        private static readonly Dictionary<string, Session> sessions
            = new Dictionary<string, Session>();

        public Request(
            Method method,
            string url,
            HeaderCollection headers,
            string body,
            Dictionary<string, string> form,
            CookieCollection cookies,
            Session session)
        {
            Method = method;
            Url = url;
            Headers = headers;
            Body = body;

            Form = form;
            Cookies = cookies;
            Session = session;
        }

        public Method Method { get; }

        public string Url { get; }

        public HeaderCollection Headers { get; }

        public string Body { get; }

        public IReadOnlyDictionary<string, string> Form { get; }

        public CookieCollection Cookies { get; }

        public Session Session { get; }

        public static Request Parse(string request)
        {
            string[] lines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);

            string[] startLine = lines[0]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string methodString = startLine[0];
            string url = startLine[1];

            Method method = ParseMethod(methodString);

            string[] headerLines = lines
                .Skip(1)
                .TakeWhile(x => x != string.Empty)
                .ToArray();

            HeaderCollection headers = ParseHeaders(headerLines);

            string[] bodyLines = lines
                .Skip(1 + headerLines.Length + 1)
                .ToArray();

            string body = string.Join("\r\n", bodyLines);

            var form = ParseForm(headers, body);
            var cookies = ParseCookies(headers);
            var session = GetSession(cookies);

            return new Request(method, url, headers, body, form, cookies, session);
        }

        private static Dictionary<string, string> ParseForm(HeaderCollection headers, string body)
        {
            var formCollection = new Dictionary<string, string>();

            if (headers.Contains(Header.ContentType) &&
                headers[Header.ContentType]
                    .StartsWith(ContentType.UrlEncoded, StringComparison.OrdinalIgnoreCase))
            {
                var parsedResult = ParseFormData(body);

                foreach (var pair in parsedResult)
                {
                    formCollection[pair.Key] = pair.Value;
                }
            }

            return formCollection;
        }

        private static Dictionary<string, string> ParseFormData(string bodyLines)
        {
            var decoded = HttpUtility.UrlDecode(bodyLines) ?? string.Empty;

            return decoded
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split('=', 2))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0], x => x[1]);
        }

        private static CookieCollection ParseCookies(HeaderCollection headers)
        {
            var cookies = new CookieCollection();

            if (!headers.Contains(Header.Cookie))
                return cookies;

            var pairs = headers[Header.Cookie]
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Select(x => x.Split('=', 2))
                .Where(x => x.Length == 2);

            foreach (var pair in pairs)
            {
                cookies.Add(new Cookie(pair[0], pair[1]));
            }

            return cookies;
        }

        private static Session GetSession(CookieCollection cookies)
        {
            string sessionId = cookies.Contains(Session.SessionCookieName)
                ? cookies[Session.SessionCookieName]
                : Guid.NewGuid().ToString();

            if (!sessions.ContainsKey(sessionId))
            {
                sessions[sessionId] = new Session(sessionId);
            }

            return sessions[sessionId];
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
            var headers = new HeaderCollection();

            foreach (var headerLine in headersLines)
            {
                string[] headerParts = headerLine
                    .Split(": ", 2, StringSplitOptions.RemoveEmptyEntries);

                if (headerParts.Length != 2)
                {
                    throw new InvalidOperationException("Invalid request header.");
                }

                headers.Add(new Header(headerParts[0], headerParts[1]));
            }

            return headers;
        }
    }
}