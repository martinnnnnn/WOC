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


namespace WOC_Server
{
    /*
        1) start listening

        2) on connect: 
            - start new thread
            - send back confirmation
            - listen
        
        3) on message: (message list)
            - decipher
            - handle
            - send to battle if needed
            - send validation response

        4) on disconnect:
            - close thread
    */

    public struct DataRecievedArgs
    {
        public DataRecievedArgs(NetworkStream stream)
        {
            Stream = stream;
        }

        public NetworkStream Stream;
    }


        public class TcpServer : IDisposable
    {
        private readonly TcpListener _listener;
        private CancellationTokenSource _tokenSource;
        private bool _listening;
        private CancellationToken _token;

        public TcpServer(IPAddress address, int port)
        {
            _listener = new TcpListener(address, port);
        }

        public bool Listening => _listening;

        public async Task StartAsync(CancellationToken? token = null)
        {
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token ?? new CancellationToken());
            _token = _tokenSource.Token;
            _listener.Start();
            _listening = true;

            try 
            {
                while (!_token.IsCancellationRequested)
                {
                    await Task.Run(async () =>
                    {
                        var tcpClientTask = _listener.AcceptTcpClientAsync();
                        var result = await tcpClientTask;

                        var stream = result.GetStream();

                        HandleClient(stream);

                        //OnDataReceived?.Invoke(this, new DataRecievedArgs(result.GetStream()));
                    }, _token);
                }
            }
            finally
            {
                _listener.Stop();
                _listening = false;
            }
        }

        private async void HandleClient(NetworkStream stream)
        {
            bool exit = false;
            while (!exit)
            {
                byte[] byteArray = new byte[1024];
                var byteCount = await stream.ReadAsync(byteArray, 0, byteArray.Length);
                var request = Encoding.UTF8.GetString(byteArray, 0, byteCount);
                Console.Write("Server : (received) " + request + "\n");
                
                var response = Encoding.UTF8.GetBytes("Who's there?");
                await stream.WriteAsync(response, 0, response.Length);

                exit = request == "exit";
            }
        }

        public void Stop()
        {
            _tokenSource?.Cancel();
            using (var client = new TcpClient())
            {
                client.Connect("127.0.0.1", 54001);
                using (var clientStream = client.GetStream())
                {
                    var request = Encoding.ASCII.GetBytes("Dummy");
                    clientStream.WriteAsync(request, 0, request.Length);
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }




    class Program
    {
        
        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            using (var server = new TcpServer(IPAddress.Any, 54001))
            {
                Console.WriteLine("Starting server...");
                var serverTask = server.StartAsync();
                serverTask.Wait();
            }

            Console.WriteLine("Any input to close cmd");
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
