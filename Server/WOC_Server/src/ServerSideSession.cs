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
        public Account account;

        public ServerSideSession(TcpClient tcpClient, Server tcpServer) : base(tcpClient)
        {
            server = tcpServer;
            account = new Account()
            {
                name = "martin",
                characters = new List<Character>()
                {
                    new Character() { name = "myhunter", type = "hunter" },
                    new Character() { name = "mychaman", type = "chaman" },
                    new Character() { name = "mysorceress", type = "sorceress" },
                    new Character() { name = "mybarbarian", type = "barbarian"}
                },
                decks = new List<Deck>()
                {
                    new Deck()
                    {
                        name = "tankdeck",
                        cards = new List<Card>()
                        {
                            new Card() { name = "attak" },
                            new Card() { name = "attack" },
                            new Card() { name = "attack" },
                            new Card() { name = "proteck" },
                            new Card() { name = "proteck" },
                            new Card() { name = "proteck" },
                            new Card() { name = "proteck" },
                            new Card() { name = "aggro" },
                            new Card() { name = "aggro" },
                            new Card() { name = "aggro" },
                            new Card() { name = "aggro" }
                        }
                    },
                    new Deck()
                    {
                        name = "heal",
                        cards = new List<Card>()
                        {
                            new Card() { name = "attak" },
                            new Card() { name = "attack" },
                            new Card() { name = "attack" },
                            new Card() { name = "proteck" },
                            new Card() { name = "proteck" },
                            new Card() { name = "proteck" },
                            new Card() { name = "proteck" },
                            new Card() { name = "heal" },
                            new Card() { name = "heal" },
                            new Card() { name = "heal" },
                            new Card() { name = "heal" }
                        }
                    }
                }
            };
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
                        case PD_InfoRequest data:
                            HandleInfoRequest(data);
                            break;
                        case PD_AccountConnect data:
                            AccountConnect(data);
                            break;
                        case PD_AccountDisconnect data:
                            AccountDisconnect(data);
                            break;
                        case PD_Chat data:
                            Broadcast(PacketData.ToJson(data)).Wait();
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

        void HandleInfoRequest(PD_InfoRequest data)
        {
            switch(data.infoType)
            {
                case "account":
                {
                    Console.WriteLine("sending account info");
                    PD_Info<Account> packet = new PD_Info<Account>()
                    {
                        info = this.account
                    };
                    string message = PacketData.ToJson(packet);

                    SendAsync(message).Wait();

                    // temp
                    //Console.WriteLine("sending account list");
                    //List<string> accNames = new List<string>();
                    //server.sessions.ForEach(session => accNames.Add(session.account.name));
                    //PD_Info<AccountList> packet2 = new PD_Info<AccountList>()
                    //{
                    //    info = new AccountList()
                    //    {
                    //        names = accNames
                    //    }
                    //};
                    //string message2 = PacketData.ToJson(packet2);

                    //SendAsync(message2).Wait();
                    break;
                }
                case "account_list":
                {
                    List<string> accNames = new List<string>();
                    server.sessions.ForEach(session => accNames.Add(session.account.name));
                    PD_Info<AccountList> packet = new PD_Info<AccountList>()
                    {
                        info = new AccountList()
                        {
                            names = accNames
                        }
                    };
                    string message = PacketData.ToJson(packet);

                    SendAsync(message).Wait();
                    break;
                }
                    
            }
        }

        void AccountConnect(PD_AccountConnect data)
        {
            account.name = data.name;
            PD_Validate packet = new PD_Validate()
            {
                validationId = data.id,
                isValid = true
                //isValid = data.name == account.name
            };
            Console.WriteLine("sending validation message");
            string message = PacketData.ToJson(packet);
            SendAsync(message).Wait();
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
    }
}
