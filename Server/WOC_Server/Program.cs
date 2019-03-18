using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

            Console.WriteLine("TCP listener and proxy. Default mode is \"text\".");
            Console.WriteLine();
            Console.WriteLine("|--- \"/exit\" to exit.                                  ---|");
            Console.WriteLine("|--- \"/show_text\" to display tcp data as text.         ---|");
            Console.WriteLine("|--- \"/hide_text\" to stop displaying tcp data as text. ---|");
            Console.WriteLine("|--- \"/drop_all\" to drop all connections.              ---|");
            Console.WriteLine("|--- \"/load accounts\" to drop all connections.         ---|");
            Console.WriteLine("|--- \"/save accounts\" to drop all connections.         ---|");
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
                    else if (line == "/load accounts")
                    {
                        string exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        server.LoadAccounts(exeDir + "/accounts.json");
                    }
                    else if (line == "/save accounts")
                    {
                        string exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        server.SaveAccounts(exeDir + "/accounts.json");
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
