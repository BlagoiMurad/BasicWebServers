using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server
{
    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener listener;

        public HttpServer(string address, int port)
        {
            this.ipAddress = IPAddress.Parse(address);
            this.port = port;

            this.listener = new TcpListener(this.ipAddress, this.port);
        }

        public void Start()
        {
            this.listener.Start();

            while (true)
            {
                TcpClient client = this.listener.AcceptTcpClient();

                using NetworkStream networkStream = client.GetStream();

                string requestText = ReadRequest(networkStream);
                Console.WriteLine(requestText);

                WriteResponse(networkStream, "Hello from the server!");

                client.Close();
            }
        }

        private static void WriteResponse(NetworkStream networkStream, string message)
        {
            byte[] responseBodyBytes = Encoding.UTF8.GetBytes(message);
            int responseBodyLength = responseBodyBytes.Length;

            string response =
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/plain; charset=UTF-8\r\n" +
                $"Content-Length: {responseBodyLength}\r\n" +
                "\r\n" +
                message;

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);

            networkStream.Write(responseBytes, 0, responseBytes.Length);
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
