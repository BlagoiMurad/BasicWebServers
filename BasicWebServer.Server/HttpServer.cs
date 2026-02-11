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
        private readonly RoutingTable routingTable;
        public HttpServer(string address, int port, Action<IRoutingTable> routingTableConfiguration)
        {
            this.ipAddress = IPAddress.Parse(address);
            this.port = port;

            this.listener = new TcpListener(this.ipAddress, this.port);
            routingTableConfiguration(this.routingTable = new RoutingTable());
        }

        public HttpServer(int port, Action<IRoutingTable> routingTable)
            :this("127.0.1", port, routingTable)
        {

        }
        public HttpServer( Action<IRoutingTable> routingTable)
           : this(8080,  routingTable)
        {

        }

        public void Start()
        {
            this.listener.Start();

            while (true)
            {
                TcpClient client = this.listener.AcceptTcpClient();

                using NetworkStream networkStream = client.GetStream();

                string requestString = ReadRequest(networkStream);
                Console.WriteLine(requestString);
                var request = Request.Parse(requestString);
                WriteResponse(networkStream, "Hello from the server!");

                var response = this.routingTable.MatchRequest(request);
                WriteResponse(networkStream, response);
                 client.Close();
            }
        }

        public static void WriteResponse(NetworkStream networkStream, Response response)
        {
            var responseBytes = Encoding.UTF8.GetBytes(response.ToString());
            networkStream.Write(res);
        }

        private static string ReadRequest(NetworkStream networkStream)
        {
            byte[] buffer = new byte[1024];

            StringBuilder request = new StringBuilder();

            int bytesRead;
            int totalBytesReceived = 0;

            do
            {
                bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                totalBytesReceived += bytesRead;

                if (totalBytesReceived > 10000)
                {
                    throw new InvalidOperationException("Request is too large.");
                }

                string requestPart = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                request.Append(requestPart);
            }
            while (networkStream.DataAvailable);

            return request.ToString();
        }
    }
}
