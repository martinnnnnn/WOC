using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using WOC_Network;

namespace WOC
{
    public class NetworkClient
    {
        ClientSideSession session;

        public async Task StartListener()
        {
            string host = "127.0.0.1";
            int port = 8000;

            TcpClient tcpClient = new TcpClient();
            
            await tcpClient.ConnectAsync(host, port);
            await StartHandleConnectionAsync(tcpClient);
        }

        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
        {
            session = new ClientSideSession(tcpClient);
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
                await session.SendAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to AsyncWrite : {0}", e.Message);
            }
        }

        public async Task<Guid> AccountCreate(string accname)
        {
            PD_AccountCreate packet = new PD_AccountCreate()
            {
                name = accname
            };
            string message = PacketData.ToJson(packet);

            await WriteAsync(message);
            return packet.id;
        }
    }
}

