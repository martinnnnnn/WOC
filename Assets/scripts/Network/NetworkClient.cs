//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Net.Sockets;
//using WOC_Network;
//using System.Net;

//namespace WOC
//{
//    public class NetworkClient
//    {
//        ClientSideSession session;
//        string ip;
//        int port;

//        public async Task StartListener(string ip, int port)
//        {
//            this.ip = ip;
//            this.port = port;

//            TcpClient tcpClient = new TcpClient();

//            await tcpClient.ConnectAsync(this.ip, this.port);
//            await StartHandleConnectionAsync(tcpClient);
//        }

//        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
//        {
//            session = new ClientSideSession(tcpClient);
//            var sessionTask = session.StartAsync();

//            try
//            {
//                await sessionTask;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.ToString());
//            }
//        }

//        public async Task WriteAsync(string message)
//        {
//            try
//            {
//                await session.SendAsync(message);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("Failed to AsyncWrite : {0}", e.Message);
//            }
//        }

//        public async Task<Guid> AccountCreate(string accname)
//        {
//            PD_Create<Account> packet = new PD_Create<Account>()
//            {
//                toCreate = new Account() { name = accname }
//            };
//            string message = PacketData.ToJson(packet);

//            await WriteAsync(message);
//            return packet.id;
//        }

//        public async Task AccountConnect(string accname)
//        {
//            PD_AccountConnect packet = new PD_AccountConnect()
//            {
//                name = accname
//            };
//            string message = PacketData.ToJson(packet);

//            await WriteAsync(message);
//        }

//        public async Task InfoRequest(string type)
//        {
//            PD_InfoRequest packet = new PD_InfoRequest()
//            {
//                infoType = "account"
//            };
//            string message = PacketData.ToJson(packet);

//            await WriteAsync(message);
//        }
//    }
//}

