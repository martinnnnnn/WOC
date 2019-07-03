﻿using System;
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

        static void Hello(IPacketData data)
        {
            switch (data)
            {
                case PD_Chat chat:
                    LOG.Print(chat.senderName + " : " + chat.message);
                    break;
            }
        }

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Session session = new Session();
            session.OnMessageReceived += Hello;
            string name = "anon";

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
                        if (input.StartsWith("name"))
                        {
                            name = input.Split('=')[1];
                        }
                        else
                        {
                            session.SendAsync(new PD_Chat
                            {
                                senderName = name,
                                message = input
                            }).Wait();
                        }
                        break;
                }
            }
        }
    }
}
