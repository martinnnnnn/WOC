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
    //
    public class Sessionn
    {
        public string ip;
        public int port;
        public TcpClient client = new TcpClient();
        public NetworkStream netstream;

        private CancellationTokenSource tokenSource;
        private CancellationToken token;
        private bool listening;

        public void Connect(string ip, int port)
        {
            Console.WriteLine("[NETWORK] Trying to connect to {0}:{1}", ip, port);
            if (listening)
            {
                Console.WriteLine("[NETWORK] Socket already open, closing it.");
                Close();
            }

            this.ip = ip;
            this.port = port;
            Console.WriteLine("[NETWORK] Connecting...");
            client = new TcpClient();
            try
            {
                client.Connect(ip, port);
                netstream = client.GetStream();
                Task.Run(() => StartAsync());
                Console.WriteLine("[NETWORK] Connected !");
            }
            catch(Exception e)
            {
                Console.WriteLine("[NETWORK] Failed to connect. {0}", e.Message);
            }
        }

        public async Task StartAsync()
        {
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
            token = tokenSource.Token;
            listening = true;

            try
            {
                byte[] byteArray = new byte[1024];
                while (!token.IsCancellationRequested)
                {
                    var byteCount = await netstream.ReadAsync(byteArray, 0, byteArray.Length, token);
                    var msg = Encoding.UTF8.GetString(byteArray, 0, byteCount);

                    Console.WriteLine("[NETWORK] received from server: " + msg);
                    HandleIncoming(msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[NETWORK] Lost connection to sever. " + e.Message);
            }
            finally
            {
                client.Close();
                listening = false;
                Console.WriteLine("[NETWORK] TCP socket closed.");
            }
        }

        public void Close()
        {
            Console.WriteLine("[NETWORK] Closing socket.");
            tokenSource.Cancel();
            SendAsync("closing").Wait();
        }

        public async Task SendAsync(string message)
        {
            try
            {
                var bytesMessage = Encoding.UTF8.GetBytes(message);
                await netstream.WriteAsync(bytesMessage, 0, bytesMessage.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to Send message : {0}", e.Message);
            }
        }

        protected virtual void HandleIncoming(string message) { }

        public async Task SendValidation(Guid toValidate, string errMessage = "")
        {
            await SendAsync(Serialization.ToJson(new PD_Validate(toValidate, errMessage)));
        }
    }

    class NetworkClientPlayground
    {

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            Sessionn network = new Sessionn();

            bool exit = false;
            while (!exit)
            {
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "close":
                        network.Close();
                        break;
                    case "connect":
                        network.Connect("127.0.0.1", 54001);
                        break;
                    case "exit":
                        exit = true;
                        break;
                    default:
                        network.SendAsync(input).Wait();
                        break;
                }
            }
        }
    }
}
