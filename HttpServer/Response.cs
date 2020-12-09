using System;
using System.IO;
using System.Net.Sockets;

namespace HttpServer
{
    public class Response
    {
        private byte[] data;
        private string status;
        private string mime;
        private Response(string status, string mime, byte[] data)
        {
            this.status = status;
            this.mime = mime;
            this.data = data;
        }

        public static Response From(Request request)
        {
            string file;
            if (request == null)
            {
                return NotWork("400.html", "400 Bad Request");
            }

            if(request.Type == "GET" || request.Type == "HEAD")
            {
                file = Environment.CurrentDirectory + Server.WebDirectory + request.Url;
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Exists && fileInfo.Extension.Contains("."))
                {
                    return MakeFromFile(fileInfo);
                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(file + "/");
                    if (!directoryInfo.Exists)
                    {
                        return NotWork("404.html", "404 Page Not Found!");
                    }

                    FileInfo[] files = directoryInfo.GetFiles();

                    foreach(FileInfo elem in files)
                    {
                        if (elem.Name.Contains("index.html"))
                        {
                            return MakeFromFile(elem);
                        }
                    }
                }
            }

            return NotWork("405.html", "Method Not Allowed");
        }

        private static Response MakeFromFile(FileInfo fileInfo)
        {
            FileStream fs = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            byte[] d = new byte[fs.Length];
            reader.Read(d, 0, d.Length);

            return new Response("200 OK", "text/html", d);
        }

        private static Response NotWork(string fileName, string status)
        {
            string file = Environment.CurrentDirectory + Server.MsgDirectory + fileName;
            FileInfo fileInfo = new FileInfo(file);
            FileStream fs = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            byte[] d = new byte[fs.Length];
            reader.Read(d, 0, d.Length);

            return new Response(status, "text/html", d);
        }

        public void Post(NetworkStream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            string responeStr = string.Format("{0} {1}\r\nServer: {2}\r\nContent-type: {3}; charset=utf-8\r\nAccept-Ranges: bytes\r\n" +
                "Content-Length: {4}\r\n", Server.Version, status, Server.ServerName, mime, data.Length);
            Console.WriteLine(responeStr);
            writer.WriteLine(responeStr);
            //stream.Write(responeStr);
            writer.Flush();

            stream.Write(data, 0, data.Length);
        }
    }
}
