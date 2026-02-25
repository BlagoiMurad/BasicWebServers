using System.Text;

namespace BasicWebServer.Server.HTTP
{
    public class Response
    {
        public Response(StatusCode statusCode)
        {
            this.StatusCode = statusCode;

            this.Headers = new HeaderCollection();

            Headers.Add(new Header("Server", "BasicWebServer"));

        }

        public StatusCode StatusCode { get; set; }

        public HeaderCollection Headers { get; set; } = new HeaderCollection();

        public string Body { get; set; }
        public Action<Request, Response> PreRenderAction { get; protected set; }

        public override string ToString()
        {
            var result = new StringBuilder();


            result.AppendLine($"HTTP/1.1 {(int)StatusCode} {StatusCode}");


            foreach (var header in Headers)
            {
                result.AppendLine($"{header.Name}: {header.Value}");
            }


            result.AppendLine();

            if (!string.IsNullOrWhiteSpace(Body))
            {
                result.Append(Body);
            }

            return result.ToString();
        }





    }
}
