using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class Request
    {
        public string Url { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }

        public Request()
        {

        }

        public Request(string type, string url, string host)
        {
            Url = url;
            Type = type;
            Host = host;
        }

        public static Request GetRequest(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return null;
            }
            string[] tokens = message.Split(' ');
            return new Request(tokens[0], tokens[1], tokens[3]);
        }
    }
}
