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


            Console.WriteLine(">> WOC Server");
            WOC_Network.Server server = new WOC_Network.Server();
            var listener = server.StartListenerAsync();

            string[] cmd = null;
            while(cmd == null || cmd[0] != "quit")
            {
                cmd = Console.ReadLine().Split(' ');
                Console.WriteLine("omg i'm such an CLI, i'm doing so much work here");
            }
            Console.WriteLine(">> quit CLI, waiting for connections to close");
            server.Close();
            try
            {
                listener.Wait();
            }
            catch {}

            //TcpServer tcpServer = new TcpServer(5000, true);
            //tcpServer.Run();
            //Console.ReadLine();

            //Server server = new Server();
            //bool portReady = false;

            //Console.WriteLine("TCP listener and proxy. Default mode is \"text\".");

            //string line = Console.ReadLine();
            //while (line != "/exit")
            //{
            //    if (!portReady)
            //    {
            //        portReady = server.TryStart(line);
            //    }
            //    else
            //    {
            //        if (line == "/show_text")
            //        {
            //            server.ShowText();
            //        }
            //        else if (line == "/hide_text")
            //        {
            //            server.HideText();
            //        }
            //        else if (line == "/drop_all")
            //        {
            //            server.DropAll();
            //        }
            //        else if (line == "/load accounts")
            //        {
            //            string exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //            server.LoadAccounts(exeDir + "/accounts.json");
            //        }
            //        else if (line == "/save accounts")
            //        {
            //            string exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //            server.SaveAccounts(exeDir + "/accounts.json");
            //        }
            //        else
            //        {
            //            server.SendAll(line);
            //        }
            //    }
            //    line = Console.ReadLine();
            //}
            //Console.Write("Shutting down server... ");
            //server.Shutdown();
            //Console.WriteLine("Bye.");
            //Thread.Sleep(500);
        }

    }
}
