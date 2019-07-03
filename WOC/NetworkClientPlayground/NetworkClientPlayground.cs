using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WOC_Core;

namespace Playground
{

    class NetworkClientPlayground
    {

        static void Hello(string msg)
        {
            LOG.Print("WE ARE IN THE HELLO");
        }

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Session session = new Session();
            session.OnMessageReceived += Hello;

            bool exit = false;
            while (!exit)
            {
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "close":
                        session.Close();
                        break;
                    case "connect":
                        session.Connect("127.0.0.1", 54001);
                        break;
                    case "exit":
                        exit = true;
                        break;
                    default:
                        session.SendAsync(input).Wait();
                        break;
                }
            }
        }
    }
}
