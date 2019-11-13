using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using WOC_Core;

namespace WOC_Server
{
    class Program
    {

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Task.Run(() => StartDiscoveryServer());

            TCPServer server = new TCPServer();
            Task serverTask = server.StartAsync(IPAddress.Any, 54001);

            bool exit = false;
            while (!exit)
            {
                string input = Console.ReadLine();

                switch (input)
                {
                    case "open":
                        serverTask = server.StartAsync(IPAddress.Any, 54001);
                        break;
                    case "close":
                        server.Close();
                        break;
                    case "exit":
                        exit = true;
                        break;
                    default:
                        break;
                }
            }

            server.Close();
            serverTask?.Wait();
            Console.WriteLine("Server closed, any input will end the program");
            Console.ReadLine();
        }

        static void StartDiscoveryServer()
        {
            Console.WriteLine("[DISCOVERY] Booting discovery server at port {0}", 8888);
            var Server = new UdpClient(8888);

            while (true)
            {
                var ClientEp = new IPEndPoint(IPAddress.Any, 0);
                var request = Server.Receive(ref ClientEp);
                var discovery = Serialization.FromJson<PD_Discovery>(Encoding.ASCII.GetString(request));

                Console.WriteLine("[DISCOVERY] Client looking for a server : {0}", ClientEp.Address.ToString());
                Server.Send(request, request.Length, ClientEp);
            }
        }
    }
}
