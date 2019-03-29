using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
//using MySql.Data.MySqlClient;


namespace WOC_Server
{
    public static class Utils
    {

        public static string DataPath = ".";

        public static T FromJson<T>(string jobj) where T : class
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            T obj;
            try
            {
                obj = JsonConvert.DeserializeObject<T>(jobj, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                obj = null;
            }
            return obj;
        }

        public static string ToJson<T>(T obj, bool indent = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return JsonConvert.SerializeObject(obj, settings);
        }

    }

    class Program
    {

        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Utils.DataPath = args[0];

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
