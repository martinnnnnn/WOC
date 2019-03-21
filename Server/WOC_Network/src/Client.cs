using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace WOC_Network
{
    public class Client
    {
        object _lock = new object();
        TcpClient tcpClient;
        NetworkStream netstream;

        public async Task StartListener()
        {
            string host = "127.0.0.1";
            int port = 8000;

            tcpClient = new TcpClient();
            
            await tcpClient.ConnectAsync(host, port);
            netstream = tcpClient.GetStream();
            Console.WriteLine("[Client] Starting listening for server messge");
            var task = StartReadLineAsync(tcpClient);
            if (task.IsFaulted)
            {
                task.Wait();
            }
        }

        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
        {
            Session session = new ClientSideSession(tcpClient);
            var sessionTask = session.StartAsync();

            try
            {
                await sessionTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async Task WriteAsync(string message)
        {
            try
            {
                var serverResponseBytes = Encoding.UTF8.GetBytes(message);
                await netstream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to AsyncWrite : {0}", e.Message);
            }
        }

        private async Task StartReadLineAsync(TcpClient tcpClient)
        {
            await Task.Yield();

            {
                var buffer = new byte[4096];
                while (true)
                {
                    try
                    {
                        Console.WriteLine("[Client] started listening");
                        var byteCount = await netstream.ReadAsync(buffer, 0, buffer.Length);
                        var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
                        Console.WriteLine(string.Format($"Server: {request}"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to AsyncRead : {0}", e.Message);
                        break;
                    }
                }
            }
        }
    }
}

