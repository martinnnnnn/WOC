using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkClientPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            var response = new byte[1024];

            using (var client = new TcpClient())
            {
                client.Connect("127.0.0.1", 54001);

                using (var clientStream = client.GetStream())
                {
                    bool exit = false;
                    while(!exit)
                    {
                        string requestStr = Console.ReadLine();
                        var request = Encoding.ASCII.GetBytes(requestStr);
                        clientStream.WriteAsync(request, 0, request.Length);
                        Task<int> size = clientStream.ReadAsync(response, 0, response.Length);
                        size.Wait();
                        Console.WriteLine("Server wrote : " + Encoding.UTF8.GetString(response, 0, size.Result));
                        exit = requestStr == "exit";
                    }
                }
            }
        }
    }
}
