using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Network
{
    public class TcpServer
    {
        object _lock = new Object();
        List<Task> _connections = new List<Task>();

        public async Task StartListener()
        {
            var tcpListener = TcpListener.Create(8000);
            tcpListener.Start();
            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                Console.WriteLine("[Server] Client has connected");
                var task = StartHandleConnectionAsync(tcpClient);
                if (task.IsFaulted)
                {
                    task.Wait();
                }
            }
        }

        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
        {
            var connectionTask = HandleConnectionAsync(tcpClient);

            lock (_lock)
            {
                _connections.Add(connectionTask);
            }

            try
            {
                await connectionTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                lock (_lock)
                {
                    _connections.Remove(connectionTask);
                }
            }
        }

        private async Task HandleConnectionAsync(TcpClient tcpClient)
        {
            await Task.Yield();

            using (var networkStream = tcpClient.GetStream())
            {
                try
                {
                    StreamReader reader = new StreamReader(networkStream);
                    StreamWriter writer = new StreamWriter(networkStream);
                    string request = string.Empty;
                    while(request != "quit\n")
                    {
                        var buffer = new byte[4096];
                        Console.WriteLine("[Server] Reading from client");
                        //var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                        //request = Encoding.UTF8.GetString(buffer, 0, byteCount);
                        request = await reader.ReadLineAsync();
                        Console.WriteLine("[Server] Client wrote {0}", request);
                        //var serverResponseBytes = Encoding.UTF8.GetBytes("Hello from server");
                        //await networkStream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);
                        await writer.WriteLineAsync("Hello from server\n");
                        Console.WriteLine("[Server] Response has been written");
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("[Server] Connection to client failed with error : " + e.Message);
                }
            }
        }
    }
}