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
using WOC_Battle;

namespace WOC_Server
{
    class Server
    {
        Battle battle;
        TCPServer server;
    }



    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            TCPServer server = new TCPServer();
            Task serverTask = null;

            bool exit = false;
            while (!exit)
            {
                string input = Console.ReadLine();

                switch (input)
                {
                    case "close":
                        server.Close();
                        break;
                    case "open":
                        LOG.Print("Starting server...");
                        serverTask = server.StartAsync(IPAddress.Any, 54001);
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
            LOG.Print("Server closed, any input will end the program");
            Console.ReadLine();
        }
    }
}
