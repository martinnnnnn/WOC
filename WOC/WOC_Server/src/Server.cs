using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WOC_Core;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace WOC_Server
{
    public class TCPServer
    {
        public IPAddress IP;
        public int Port;

        private TcpListener listener;
        private CancellationTokenSource tokenSource;
        private bool listening;

        private CancellationToken token;
        public SynchronizedCollection<ServerSession> sessions = new SynchronizedCollection<ServerSession>();
        public ConcurrentDictionary<string, WOC_Core.Account> users = new ConcurrentDictionary<string, WOC_Core.Account>();

        public WOC_Core.RTTS.Battle battle = null;


        public TCPServer()
        {

        }

        public void InitBattle(int randomSeed)
        {
            List<WOC_Core.RTTS.BattlePlayer> players =
                new List<WOC_Core.RTTS.BattlePlayer>(users.Select(n =>
                new WOC_Core.RTTS.BattlePlayer(battle, n.Key, new WOC_Core.RTTS.Deck()
                {
                    name = "defaultDeck",
                    cards = new List<WOC_Core.RTTS.Card>()
                    {
                                    new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
                                    new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
                                    new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
                                    new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
                                    new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
                                    new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
                                    new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
                                    new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
                                    new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
                                    new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
                                    new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
                                    new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
                                    new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
                                    new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
                                    new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
                                    new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
                                    new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
                                    new WOC_Core.RTTS.Card() { name = "card5", timeCost = 5 },
                                    new WOC_Core.RTTS.Card() { name = "card5", timeCost = 5 },
                                    new WOC_Core.RTTS.Card() { name = "card5", timeCost = 5 },
                                    new WOC_Core.RTTS.Card() { name = "card6", timeCost = 6 },
                    }
                })));

            List<WOC_Core.RTTS.Monster> monsters = new List<WOC_Core.RTTS.Monster>()
            {
                new WOC_Core.RTTS.Monster("monster", 15)
            };

            battle = new WOC_Core.RTTS.Battle(players, monsters, randomSeed);
        }

        public async Task StartAsync(IPAddress ip, int port)
        {
            Console.WriteLine("[SERVER] Welcome to the WOC Server ! ");
            Console.WriteLine("[SERVER] Starting to listen on {0}:{1}", ip, port);
            if (listening)
            {
                Console.WriteLine("[SERVER] Already started, trying to close first...");
                Close();
            }

            IP = ip;
            Port = port;
            listener = new TcpListener(IP, Port);
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
            token = tokenSource.Token;
            listener.Start();
            listening = true;
            Console.WriteLine("[SERVER] Server operational.");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Run(async () =>
                    {
                        var tcpClientTask = listener.AcceptTcpClientAsync();
                        TcpClient client = await tcpClientTask;

                        ServerSession session = new ServerSession(this);
                        session.Connect(client);
                        //Broadcast(new PD_SessionConnect { name = session.Name }, null, true).Wait();
                        session.OnDisconnect += () =>
                        {
                            sessions.Remove(session);
                            //Broadcast(new PD_SessionDisconnect { name = session.Name }, null, true).Wait();
                            Console.WriteLine("[SERVER] Client closed. {0} clients still connected", sessions.Count);
                        };
                        sessions.Add(session);
                        Console.WriteLine("[SERVER] Client connected. {0} clients connected", sessions.Count);
                    }, token);
                }
            }
            finally
            {
                Close();
            }
        }

        public void Close()
        {
            if (listening)
            {
                Console.WriteLine("[SERVER] Closing server.");
                listener.Stop();
                foreach (Session s in sessions)
                {
                    s?.Close();
                }
                sessions.Clear();
                listening = false;
                Console.WriteLine("[SERVER] Server closed.");
            }
            else
            {
                Console.WriteLine("[SERVER] already closed.");
            }
        }

        public void Broadcast(IPacketData data, Session toIgnore = null)
        {
            foreach (Session session in sessions)
            {
                if (toIgnore == null || session != toIgnore)
                {
                    session.Send(data);
                }
            }
        }

        public void BroadcastTo(IPacketData data, IEnumerable<ServerSession> sessions)
        {
            foreach (Session session in sessions)
            {
                 session.Send(data);
            }
        }
    }
}