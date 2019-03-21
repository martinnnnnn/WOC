using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WOC_Network;

namespace WOC_Server
{
    public class ServerSideSession : Session
    {
        public Server server;

        public ServerSideSession(TcpClient tcpClient, Server tcpServer) : base(tcpClient)
        {
            server = tcpServer;
        }

        public async Task Broadcast(string message)
        {
            var tasks = server.sessions.Select(session => session.SendAsync(message));
            await Task.WhenAll(tasks);

        }

        protected override void HandleIncoming(string message)
        {
            Console.WriteLine("I'm handling {0}", message);
            try
            {
                IPacketData packet = PacketData.FromJson(message);

                if (packet != null)
                {
                    switch (packet)
                    {
                        case PD_AccountCreate data:
                            AccountCreate(data);
                            break;
                        case PD_AccountConnect data:
                            AccountConnect(data);
                            break;
                        case PD_AccountDisconnect data:
                            AccountDisconnect(data);
                            break;
                        case PD_AccountList data:
                            //AccountList(data);
                            break;
                        case PD_CharacterCreate data:
                            CharacterCreate(data);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Unknow JSON message : " + message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error while parsing JSON message : " + message);
            }
        }

        void AccountCreate(PD_AccountCreate data)
        {
            account = new Account()
            {
                name = data.name
            };
        }

        void AccountConnect(PD_AccountConnect data)
        {
            account = new Account()
            {
                name = data.name
            };
            // load info from db
        }

        void AccountDisconnect(PD_AccountDisconnect data)
        {
            if (account != null)
            {
                // save info to db
                account = null;
            }
        }

        void CharacterCreate(PD_CharacterCreate data)
        {
            if (account != null)
            {
                //account.characters.Add(new Character()
                //{
                //    name = data.name,
                //    type = data.type
                //});
            }
        }
    }
}
