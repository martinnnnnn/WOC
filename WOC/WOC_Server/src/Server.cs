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
    public class GameServer
    {
        public IPAddress IP;
        public int Port;

        private TcpListener listener;
        private CancellationTokenSource tokenSource;
        private bool listening;

        private CancellationToken token;
        public SynchronizedCollection<ServerSession> sessions = new SynchronizedCollection<ServerSession>();
        public ConcurrentDictionary<string, WOC_Core.Account> users = new ConcurrentDictionary<string, WOC_Core.Account>();

        public Battle battle;


        public GameServer()
        {

        }

        public void InitBattle()
        {
            List<BattlePlayer> players =
                new List<BattlePlayer>(users.Select((n, i) =>
                {
                    return new BattlePlayer(n.Key, i, new Deck()
                    {
                        name = "defaultDeck",
                        cards = new List<Card>()
                        {
                            new Card() { name = "card1a", timeCost = 1 },
                            new Card() { name = "card1b", timeCost = 1 },
                            new Card() { name = "card1c", timeCost = 1 },
                            new Card() { name = "card1d", timeCost = 1 },
                            new Card() { name = "card2a", timeCost = 2 },
                            new Card() { name = "card2b", timeCost = 2 },
                            new Card() { name = "card2c", timeCost = 2 },
                            new Card() { name = "card2d", timeCost = 2 },
                            new Card() { name = "card3a", timeCost = 3 },
                            new Card() { name = "card3b", timeCost = 3 },
                            new Card() { name = "card3c", timeCost = 3 },
                            new Card() { name = "card3d", timeCost = 3 },
                            new Card() { name = "card3e", timeCost = 3 },
                            new Card() { name = "card4a", timeCost = 4 },
                            new Card() { name = "card4b", timeCost = 4 },
                            new Card() { name = "card4c", timeCost = 4 },
                            new Card() { name = "card4d", timeCost = 4 },
                            new Card() { name = "card5a", timeCost = 5 },
                            new Card() { name = "card5b", timeCost = 5 },
                            new Card() { name = "card5c", timeCost = 5 },
                            new Card() { name = "card6a", timeCost = 6 },
                        }
                    });
                }));

            List<Monster> monsters = new List<Monster>()
            {
                new Monster("monster", 15.0)
            };

            battle = new Battle(this, players, monsters);

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                battle.PlayersTurnStart();
            });
        }

        public PD_BattleState GetBattleState(string playerName)
        {
            BattlePlayer mainPlayer = battle.players.Find(p => p.name == playerName);

            return new PD_BattleState()
            {
                monsters = battle.monsters.Select(m =>
                {
                    return new PD_BattleStateMonster()
                    {
                        location = 0,
                        name = m.name
                    };
                }).ToList(),

                players = battle.players.Select(p =>
                {
                    return new PD_BattleStatePlayer()
                    {
                        location = p.location,
                        name = p.name,
                        life = 12,
                        handCount = p.hand.Count,
                        drawPileCount = p.drawPile.Count,
                        discardPileCount = p.discardPile.Count
                    };
                }).Where(p => p.name != playerName).ToList(),

                mainPlayer = new PD_BattleStateMainPlayer()
                {
                    location = mainPlayer.location,
                    life = 12,
                    hand = mainPlayer.hand.Cards.Select(c => c.name).ToList(),
                    drawPileCount = mainPlayer.drawPile.Count,
                    discardPileCount = mainPlayer.discardPile.Count
                }
            };
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