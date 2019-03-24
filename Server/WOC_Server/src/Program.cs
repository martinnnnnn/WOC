using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
            using MySql.Data.MySqlClient;


namespace WOC_Server
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine(">> WOC Server");
            Server server = new Server();
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
        }

    }
}
