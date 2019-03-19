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
    public class SocketInfo
    {
        public byte[] buffer = new byte[255];
        public Socket socket = null;
    }

    class Program
    {
        private static string address;
        private static int port;
        private static IPEndPoint clientEndpoint;
        private static string name = "default name";
        //private static Socket clientSocket;
        //private static byte[] buffer = new byte[255];

        private static List<SocketInfo> socketList = new List<SocketInfo>();

        static void Start()
        {
            /*IPHostEntry localMachineInfo =
                Dns.GetHostEntry(Dns.GetHostName());*/
            /*IPEndPoint myEndpoint = new IPEndPoint(
                localMachineInfo.AddressList[0], port);*/
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SocketInfo info = new SocketInfo();
            info.socket = clientSocket;
            lock (socketList)
            {
                socketList.Add(info);
            }
            Console.Write("Connecting... ");
            try
            {
                info.socket.Connect(clientEndpoint);
                Console.WriteLine("Done.");

                info.socket.BeginReceive(info.buffer, 0, info.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), info);
            }
            catch (Exception e)
            {
                lock (socketList)
                {
                    socketList.Remove(info);
                }
                Console.WriteLine("Fail.");
                Console.WriteLine(e);
            }
        }

        static void ReceiveCallback(IAsyncResult result)
        {
            SocketInfo info = (SocketInfo)result.AsyncState;
            try
            {
                int bytestoread = info.socket.EndReceive(result);
                if (bytestoread > 0)
                {
                    string text = Encoding.UTF8.GetString(info.buffer, 0, bytestoread);
                    Console.Write(text);
                    info.socket.BeginReceive(info.buffer, 0, 255, SocketFlags.None, new AsyncCallback(ReceiveCallback), info);
                }
                else
                {
                    lock (socketList)
                    {
                        socketList.Remove(info);
                    }
                    Console.WriteLine("Client finished normally.");
                    info.socket.Close();
                }
            }
            catch (Exception e)
            {
                lock (socketList)
                {
                    socketList.Remove(info);
                }
                Console.WriteLine("Client Disconnected.");
                Console.WriteLine(e);
            }
        }

        static object pingThreadLock = new object();
        static bool pingWorking = false;

        static void PingThread()
        {
            bool working = true;
            while (working)
            {
                Thread.Sleep(3000);

                try
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(name + " : Ping!\n");
                    lock (socketList)
                    {
                        foreach (SocketInfo si in socketList)
                        {
                            si.socket.Send(bytes, bytes.Length,
                                SocketFlags.None);
                            Console.WriteLine("Ping OK!");
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Unable to send data. Connection lost.");
                    lock (pingThreadLock)
                    {
                        pingWorking = false;
                    }
                }

                Thread.Sleep(10);
                lock (pingThreadLock)
                {
                    working = pingWorking;
                }
            }
            Console.WriteLine("Ping thread stopped.");
        }

        static void SendData(string data)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data + "\n");
                lock (socketList)
                {
                    foreach (SocketInfo info in socketList)
                    {
                        info.socket.Send(bytes, bytes.Length, SocketFlags.None);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Unable to send data. Connection lost.");
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("TCP client.");
            Console.WriteLine();
            Console.WriteLine("|--- \"exit\" to exit.                                  ---|");
            Console.WriteLine("|--- \"start ping\" to start pinging server.            ---|");
            Console.WriteLine("|--- \"stop ping\" to stop pinging server.              ---|");
            Console.WriteLine("|--- \"start {cn}\" to start {cn} number of connections.---|");
            Console.WriteLine("|--- \"setname <name>\" to set your name.---|");
            Console.WriteLine();
            Console.Write("Please enter remote address and port: ");
            bool portReady = false;
            string line = Console.ReadLine();
            while (line != "exit")
            {
                if (!portReady)
                {
                    try
                    {
                        string[] ss = line.Split(':');
                        port = int.Parse(ss[1]);
                        IPAddress add = IPAddress.Parse(ss[0]);
                        clientEndpoint = new IPEndPoint(add, port);
                        address = clientEndpoint.Serialize().ToString();
                        if (port > short.MaxValue || port < 2)
                        {
                            Console.WriteLine("Invalid port.");
                            Console.Write("Please enter remote address and port: ");
                        }
                        else
                        {
                            Start();
                            portReady = true;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Invalid address.");
                        Console.Write("Please enter remote address and port: ");
                    }
                }
                else
                {
                    if (line == "start ping")
                    {
                        bool lping = false;
                        lock (pingThreadLock)
                        {
                            lping = pingWorking;
                        }
                        if (!lping)
                        {
                            pingWorking = true;
                            Thread th = new Thread(new ThreadStart(PingThread));
                            Console.WriteLine("Ping thread started.");
                            th.Start();
                        }
                    }
                    else if (line == "stop ping")
                    {
                        bool lping = false;
                        lock (pingThreadLock)
                        {
                            lping = pingWorking;
                        }
                        if (lping)
                        {
                            lock (pingThreadLock)
                            {
                                pingWorking = false;
                            }
                        }
                    }
                    else if (line.StartsWith("start "))
                    {
                        int count = 0;
                        try
                        {
                            count = int.Parse(line.Substring("start ".Length));
                        }
                        catch { }
                        Console.WriteLine("Starting " + count + " connections.");
                        for (int i = 0; i < count; i++)
                        {
                            Start();
                        }
                    }
                    else if (line.StartsWith("setname "))
                    {
                        name = line.Substring("setname ".Length);
                    }
                    else if (line.StartsWith("account_list"))
                    {
                        SendData(PacketData.ToJson(new PacketDataAccountList()));
                    }
                    else if (line.StartsWith("account_create"))
                    {
                        string[] cmd = line.Split(' ');
                        if (cmd.Length >= 3)
                        {
                            SendData(PacketData.ToJson(new PacketDataAccountCreate()
                            {
                                name = cmd[1],
                                password = cmd[2]
                            }));
                        }
                    }
                    else if (line.StartsWith("account_connect"))
                    {
                        string[] cmd = line.Split(' ');
                        if (cmd.Length >= 3)
                        {
                            SendData(PacketData.ToJson(new PacketDataAccountConnect()
                            {
                                name = cmd[1],
                                password = cmd[2]
                            }));
                        }
                    }
                    else if (line.StartsWith("account_disconnect"))
                    {
                        string[] cmd = line.Split(' ');
                        if (cmd.Length >= 3)
                        {
                            SendData(PacketData.ToJson(new PacketDataAccountDisconnect()
                            {
                                name = cmd[1],
                                password = cmd[2]
                            }));
                        }
                    }
                    else
                    {
                        SendData(line);
                    }
                }
                line = Console.ReadLine();
            }
            Console.Write("Shutting down client... ");
            try
            {
                lock (socketList)
                {
                    for (int i = socketList.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            socketList[i].socket.Close();
                        }
                        catch { }
                        socketList.RemoveAt(i);
                    }
                }
            }
            catch { }
            Console.WriteLine("Bye.");
            Thread.Sleep(500);
        }
    }
}
