using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Network
{
    public class Connection
    {
        public TcpClient client;
        public NetworkStream netstream;
        public Action<string> HandleIncomingMessage;

        public Connection(TcpClient tcpClient)
        {
            client = tcpClient;
            netstream = client.GetStream();
        }

        public async Task HandleConnectionAsync()
        {
            //await Task.Yield();

            var buffer = new byte[4096];
            Console.WriteLine("[Server] Reading from client");
            while (true)
            {
                try
                {
                    var byteCount = await netstream.ReadAsync(buffer, 0, buffer.Length);
                    var request = Encoding.UTF8.GetString(buffer, 0, byteCount);

                    HandleIncomingMessage(request);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to AsyncRead : {0}", ex.Message);
                    break;
                }
            }
        }

        public async Task Send(string msg)
        {
            try
            {
                var bytesMessage = Encoding.UTF8.GetBytes(msg);
                await netstream.WriteAsync(bytesMessage, 0, bytesMessage.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to Send message : {0}", e.Message);
            }
        }
    }
}
