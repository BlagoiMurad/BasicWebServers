namespace BasicWebServer.Server.HTTP
{
    public class Response
    {
        public Response(StatusCode statusCode)
        {
            this.StatusCode = statusCode;

            this.Headers = new HeaderCollection();

            this.Headers.Add(new Header("Server", "BasicWebServer"));
            this.Headers.Add(new Header("Content-Type", "text/plain; charset=UTF-8"));
        }

        public StatusCode StatusCode { get; }

        public HeaderCollection Headers { get; }

        public string Body { get; set; }
    }
}
