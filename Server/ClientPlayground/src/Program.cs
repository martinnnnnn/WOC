using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using WOC_Network;

namespace ConnectionClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            var clientTask = client.StartListener();

            string userInput = string.Empty;
            while (userInput != "quit")
            {
                userInput = Console.ReadLine();
                client.WriteAsync(userInput).Wait();
            }
            clientTask.Wait();
        }
    }
}
