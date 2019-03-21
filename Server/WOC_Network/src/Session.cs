using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Network
{
    public class Session
    {
        public Connection connection = null;
        public Account account = null;

        public Session(TcpClient tcpClient)
        {
            connection = new Connection(tcpClient);
            connection.HandleIncomingMessage = HandleIncoming;
        }

        public async Task SendAsync(string message)
        {
            await connection.Send(message);
        }

        public async Task StartAsync()
        {
            await Task.Yield();
            await connection.HandleConnectionAsync();
        }

        protected virtual void HandleIncoming(string message)
        {
            Console.WriteLine("I'm don't know how to handle {0}", message);
        }
    }

    public class ClientSideSession : Session
    {
        public ClientSideSession(TcpClient tcpClient) : base(tcpClient)
        {
        }
    }

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
