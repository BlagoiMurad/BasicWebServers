namespace BasicWebServer.Server.HTTP
{
    public class Response
    {
        public Response(StatusCode statusCode)
        {
            this.StatusCode = statusCode;

            this.Headers = new HeaderCollection();

            this.Headers.Add(Header.Server, "BasicWebServer");
            this.Headers.Add(Header.ContentType, "text/plain; charset=UTF-8");
        }

        public StatusCode StatusCode { get; }

        public override string? ToString()
        {
            var result = new System.Text.StringBuilder();
            result.Append($"HTTP/1.1 {(int)StatusCode} {StatusCode}");

            foreach (var header in Headers)
            {
                result.Append($"{header.Name} : {header.Value}");
            }
            result.AppendLine();
            if(string.IsNullOrEmpty(Body) == false)
            {
                result.Append(Body);
            }
            return result.ToString();
        }

        public HeaderCollection Headers { get; }

        public string Body { get; set; }
    }
}
