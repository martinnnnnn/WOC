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

namespace WOC_Server
{
    public class TcpServer
    {
        private readonly TcpListener listener;
        private CancellationTokenSource tokenSource;
        private bool listening;
        private CancellationToken token;

        private SynchronizedCollection<Session> sessions = new SynchronizedCollection<Session>();
        //private SynchronizedCollection<TcpClient> clients = new SynchronizedCollection<TcpClient>();
        public TcpServer(IPAddress address, int port)
        {
            listener = new TcpListener(address, port);
        }

        void IncomingHandling(string msg)
        {
            Console.Write("[SERVER] received : " + msg + "\n");

            var tasks = new List<Task>();
            try
            {
                foreach (Session s in sessions)
                {
                    tasks.Add(Task.Run(async () => { await s.SendAsync(msg); }));
                }
                Task.WaitAll(tasks.ToArray(), 10000);
            }
            catch (Exception)
            {
                LOG.Print("[SERVER] Failed to broadcast message");
            }
        }

        public async Task StartAsync()
        {
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
            token = tokenSource.Token;
            listener.Start();
            listening = true;

            try 
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Run(async () =>
                    {
                        var tcpClientTask = listener.AcceptTcpClientAsync();
                        TcpClient client = await tcpClientTask;

                        Session session = new Session();
                        session.Connect(client);
                        sessions.Add(session);
                        LOG.Print("[SERVER] Client connected. {0} clients connected", sessions.Count);
                        session.OnMessageReceived += IncomingHandling;
                        session.OnDisconnect += () =>
                        {
                            sessions.Remove(session);
                            LOG.Print("[SERVER] Client closed. {0} clients still connected", sessions.Count);

                        };
                        //HandleClient(client);
                    }, token);
                }
            }
            finally
            {
                LOG.Print("[SERVER] Closing server.");
                listener.Stop();
                listening = false;
                LOG.Print("[SERVER] Server closed.");
            }
        }

        //private async void HandleClient(TcpClient client)
        //{
        //    var stream = client.GetStream();
        //    sessions.Add(client);

        //    LOG.Print("[SERVER] Client connected. {0} clients connected", sessions.Count);

        //    bool exit = false;
        //    try
        //    {
        //        while (!exit)
        //        {
        //            byte[] byteArray = new byte[1024];
        //            var byteCount = await stream.ReadAsync(byteArray, 0, byteArray.Length);
        //            var request = Encoding.UTF8.GetString(byteArray, 0, byteCount);
        //            Console.Write("[SERVER] received : " + request + "\n");
        //            var tasks = new List<Task>();
        //            foreach (TcpClient c in sessions)
        //            {
        //                tasks.Add(Task.Run(() =>
        //                {
        //                    if (c != client)
        //                    {
        //                        c.GetStream().WriteAsync(byteArray, 0, byteCount);
        //                    }
        //                    else
        //                    {
        //                        byte[] echo = Encoding.UTF8.GetBytes("Message Received!");
        //                        c.GetStream().WriteAsync(echo, 0, echo.Length);
        //                    }
        //                }));
        //            }
        //            Task.WaitAll(tasks.ToArray(), 10000);
        //            exit = request == "exit";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        LOG.Print("[SERVER] Client disconnecting.");
        //    }
        //    finally
        //    {
        //        LOG.Print("[SERVER] Closing client.");
        //        sessions.Remove(client);
        //        client.Close();
        //        LOG.Print("[SERVER] Client Closed. {0} clients still connected", sessions.Count);
        //    }

        //}

        public void Stop()
        {
            if (listening)
            {
                LOG.Print("Closing server");
                tokenSource?.Cancel();
                using (var closer = new TcpClient())
                {
                    closer.Connect("127.0.0.1", 54001);
                    closer.Close();
                }
            }
            else
            {
                LOG.Print("Server already closed");
            }
        }
    }
    

    class Program
    {
        
        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            TcpServer server = null;
            Task serverTask = null;

            bool exit = false;
            while (!exit)
            {
                string input = Console.ReadLine();

                switch (input)
                {
                    case "close":
                        server.Stop();
                        break;
                    case "open":
                        LOG.Print("Starting server...");
                        server = new TcpServer(IPAddress.Any, 54001);
                        serverTask = server.StartAsync();
                        break;
                    case "exit":
                        exit = true;
                        break;
                    default:
                        break;
                }
            }

            server?.Stop();
            serverTask?.Wait();
            LOG.Print("Server closed, any input will end the program");
            Console.ReadLine();

            //LOG.Print(">> WOC Server");
            //Server server = new Server();
            //var listener = server.StartListenerAsync();

            //string[] cmd = null;
            //while (cmd == null || cmd[0] != "quit")
            //{
            //    cmd = Console.ReadLine().Split(' ');
            //    LOG.Print("omg i'm such an CLI, i'm doing so much work here");
            //}
            //LOG.Print(">> quit CLI, waiting for connections to close");
            //server.Close();
            //try
            //{
            //    listener.Wait();
            //}
            //catch {}
        }
    }
}
