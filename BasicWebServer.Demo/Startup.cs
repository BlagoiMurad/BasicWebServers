using BasicWebServer.Server;
using BasicWebServer.Server.Responses;

namespace BasicWebServer.Demo
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            var server = new HttpServer(routes =>
            {   routes.MapGet("/", new TextResponse("Hello from the server!"));
                routes.MapGet("/about", new TextResponse("This is a demo of the BasicWebServer."));
                routes.MapPost("/submit", new TextResponse("Data submitted successfully!"));
            }
            );
            server.Start();
        }
    }
}
