using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WOC_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            bool portReady = false;


            string name = "martin";
            string password = "hello";
            List<bool> stuff = new List<bool>() { false, true, false, false, true };
            Dictionary<int, bool> stufff = new Dictionary<int, bool>()
            {
                { 3, true },
                { 12, false },
                { 16, false },
                { 13, false },
                { 10, true },
            };

            WOC_Network.Packet packet = new WOC_Network.Packet()
            {
                type = "account_create",
                data = new Dictionary<string, object>()
                {
                    { "name", name },
                    { "password", password },
                    { "stuff", stuff },
                    { "stufff", stufff }
                }
            };

            string hello = JsonConvert.SerializeObject(packet, Formatting.Indented);
            Console.WriteLine(hello);

            Console.WriteLine("TCP listener and proxy. Default mode is \"text\".");
            Console.WriteLine();
            Console.WriteLine("|--- \"/exit\" to exit.                                  ---|");
            Console.WriteLine("|--- \"/show_text\" to display tcp data as text.         ---|");
            Console.WriteLine("|--- \"/hide_text\" to stop displaying tcp data as text. ---|");
            Console.WriteLine("|--- \"/drop_all\" to drop all connections.              ---|");
            Console.WriteLine();
            Console.Write("Please enter port number: ");

            string line = Console.ReadLine();
            while (line != "/exit")
            {
                if (!portReady)
                {
                    portReady = server.TryStart(line);
                }
                else
                {
                    if (line == "/show_text")
                    {
                        server.ShowText();
                    }
                    else if (line == "/hide_text")
                    {
                        server.HideText();
                    }
                    else if (line == "/drop_all")
                    {
                        server.DropAll();
                    }
                    else
                    {
                        server.SendAll(line);
                    }
                }
                line = Console.ReadLine();
            }
            Console.Write("Shutting down server... ");
            server.Shutdown();
            Console.WriteLine("Bye.");
            Thread.Sleep(500);
        }

    }
}
