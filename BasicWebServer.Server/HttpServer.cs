using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BasicWebServer.Server.Contracts;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Routing;

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
                this.ipAddress = IPAddress.Parse(address);
                this.port = port;
                this.listener = new TcpListener(this.ipAddress, this.port);

                routingTableConfiguration(this.routes = new RoutingTable());
            }


            public HttpServer(int port, Action<IRoutingTable> routingTable) : this("127.0.0.1", port, routingTable)
            {

            }
            public HttpServer(Action<IRoutingTable> routingTable) : this(8081, routingTable)
            {

            }


            public async Task Start()
            {
                this.listener.Start();

                Console.WriteLine($"Server started on port {port}. ");
                Console.WriteLine("Listening for requests...");

                while (true)
                {
                    TcpClient client = await this.listener.AcceptTcpClientAsync();

                    _ = Task.Run(async () =>
                    {
                        using NetworkStream networkStream = client.GetStream();

                        var requestText = await ReadRequestAsync(networkStream);
                        Console.WriteLine(requestText);
                        var request = Request.Parse(requestText);

                        // client.Close();

                        var response = routes.MatchRequest(request);

                        if (response.PreRenderAction != null)
                        {
                            response.PreRenderAction(request, response);
                        }
                        await WriteResponseAsync(networkStream, response);

                        client.Close();
                    });


                }
            }

            private async Task WriteResponseAsync(NetworkStream networkStream, Response response)
            {
                var responseBytes = Encoding.UTF8.GetBytes(response.ToString());

                await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                await networkStream.FlushAsync();
            }
            private async Task<string> ReadRequestAsync(NetworkStream networkStream)
            {
                var buffer = new byte[8192];
                var requestBuilder = new StringBuilder();

                int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                }

                return requestBuilder.ToString();
            }
        }


    }

