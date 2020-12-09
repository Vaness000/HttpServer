using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HttpServer
{
    public class Server
    {
        private TcpListener listener;
        private bool running = false;
        public const string ServerName = "MyHttpServer/1.1";
        public const string Version = "HTTP/1.1";
        public const string MsgDirectory = "/root/msg/";
        public const string WebDirectory = "/root/web/";

        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void StartServer()
        {
            Thread thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }

        private void Run()
        {
            listener.Start();
            running = true;
            Console.WriteLine("Сервер запущен.");

            while (running)
            {
                Console.WriteLine("Ожидание подключений");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Клиент подключен");
                HandleClient(client);
                client.Close();

            }

        }

        private void HandleClient(TcpClient client)
        {
            using(StreamReader reader = new StreamReader(client.GetStream()))
            {
                StringBuilder message = new StringBuilder();
                while(reader.Peek() != -1)
                {
                    message.Append(string.Format("{0}\n", reader.ReadLine()));
                }

                Console.WriteLine("REQUEST: \r\n{0}", message);
                Request request = Request.GetRequest(message.ToString());
                Response response = Response.From(request);
                response.Post(client.GetStream());
            }
        }
    }
}
