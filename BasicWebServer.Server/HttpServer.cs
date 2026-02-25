using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BasicWebServer.Server.Contracts;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Routing;
using Cookie = BasicWebServer.Server.HTTP.Cookie;
using ServerCookie = BasicWebServer.Server.HTTP.Cookie;

namespace BasicWebServer.Server
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener listener;

        private readonly RoutingTable routes;

        public HttpServer(string address, int port, Action<IRoutingTable> routingTableConfiguration)
        {
            ipAddress = IPAddress.Parse(address);
            this.port = port;

            listener = new TcpListener(ipAddress, this.port);
            routingTableConfiguration(routes = new RoutingTable());
        }

        public HttpServer(int port, Action<IRoutingTable> routingTable)
            : this("127.0.0.1", port, routingTable) { }

        public HttpServer(Action<IRoutingTable> routingTable)
            : this(8081, routingTable) { }

        public async Task Start()
        {
            listener.Start();

            Console.WriteLine($"Server started on port {port}.");
            Console.WriteLine("Listening for requests...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();

                _ = Task.Run(async () =>
                {
                    try
                    {
                        using (client)
                        using (var networkStream = client.GetStream())
                        {
                            var requestText = await ReadRequestAsync(networkStream);

                            if (string.IsNullOrWhiteSpace(requestText))
                                return;

                            var request = Request.Parse(requestText);

                            var response = routes.MatchRequest(request);

                            // ✅ 04: session трябва да се добавя към всеки response
                            AddSession(request, response);

                            if (response.PreRenderAction != null)
                                response.PreRenderAction(request, response);

                            await WriteResponseAsync(networkStream, response);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
            }
        }

        private static void AddSession(Request request, Response response)
        {
            var session = request.Session;

            if (!session.ContainsKey(Session.CurrentDateKey))
            {
                session[Session.CurrentDateKey] = DateTime.UtcNow.ToString("O");
            }

            response.Cookies.Add(new Cookie(Session.SessionCookieName, session.Id));
        }

        private static async Task WriteResponseAsync(NetworkStream networkStream, Response response)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response.ToString());

            await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            await networkStream.FlushAsync();
        }

        private static async Task<string> ReadRequestAsync(NetworkStream networkStream)
        {
            var buffer = new byte[8192];
            var requestBuilder = new StringBuilder();

            int totalBytesReceived = 0;
            int bytesRead;

            do
            {
                bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                totalBytesReceived += bytesRead;

                if (totalBytesReceived > 10000)
                    throw new InvalidOperationException("Request is too large.");

                if (bytesRead > 0)
                    requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }
            while (networkStream.DataAvailable);

            return requestBuilder.ToString();
        }
    }
}