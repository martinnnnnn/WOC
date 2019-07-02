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

namespace WOC_Server
{
    public class TcpServer
    {
        private readonly TcpListener listener;
        private CancellationTokenSource tokenSource;
        private bool listening;
        private CancellationToken token;

        private SynchronizedCollection<TcpClient> clients = new SynchronizedCollection<TcpClient>();
        public TcpServer(IPAddress address, int port)
        {
            listener = new TcpListener(address, port);
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
                        var result = await tcpClientTask;
                        HandleClient(result);
                    }, token);
                }
            }
            finally
            {
                Console.WriteLine("[SERVER] Closing server.");
                listener.Stop();
                listening = false;
                Console.WriteLine("[SERVER] Server closed.");
            }
        }

        private async void HandleClient(TcpClient client)
        {
            var stream = client.GetStream();
            clients.Add(client);

            Console.WriteLine("[SERVER] Client connected. {0} clients connected", clients.Count);

            bool exit = false;
            try
            {
                while (!exit)
                {
                    byte[] byteArray = new byte[1024];
                    var byteCount = await stream.ReadAsync(byteArray, 0, byteArray.Length);
                    var request = Encoding.UTF8.GetString(byteArray, 0, byteCount);
                    Console.Write("[SERVER] received : " + request + "\n");
                    var tasks = new List<Task>();
                    foreach (TcpClient c in clients)
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            if (c != client)
                            {
                                c.GetStream().WriteAsync(byteArray, 0, byteCount);
                            }
                            else
                            {
                                byte[] echo = Encoding.UTF8.GetBytes("Message Received!");
                                c.GetStream().WriteAsync(echo, 0, echo.Length);
                            }
                        }));
                    }
                    Task.WaitAll(tasks.ToArray(), 10000);
                    exit = request == "exit";
                }
            }
            catch (Exception)
            {
                Console.WriteLine("[SERVER] Client disconnecting.");
            }
            finally
            {
                Console.WriteLine("[SERVER] Closing client.");
                clients.Remove(client);
                client.Close();
                Console.WriteLine("[SERVER] Client Closed. {0} clients still connected", clients.Count);
            }

        }

        public void Stop()
        {
            if (listening)
            {
                Console.WriteLine("Closing server");
                tokenSource?.Cancel();
                using (var closer = new TcpClient())
                {
                    closer.Connect("127.0.0.1", 54001);
                    closer.Close();
                }
            }
            else
            {
                Console.WriteLine("Server already closed");
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
                        Console.WriteLine("Starting server...");
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
            Console.WriteLine("Server closed, any input will end the program");
            Console.ReadLine();

            //Console.WriteLine(">> WOC Server");
            //Server server = new Server();
            //var listener = server.StartListenerAsync();

            //string[] cmd = null;
            //while (cmd == null || cmd[0] != "quit")
            //{
            //    cmd = Console.ReadLine().Split(' ');
            //    Console.WriteLine("omg i'm such an CLI, i'm doing so much work here");
            //}
            //Console.WriteLine(">> quit CLI, waiting for connections to close");
            //server.Close();
            //try
            //{
            //    listener.Wait();
            //}
            //catch {}
        }
    }
}
