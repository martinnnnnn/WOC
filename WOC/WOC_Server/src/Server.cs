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
            Random rand = new Random();

            List<Player> players =
                new List<Player>(users.Select((n, i) =>
                {
                    return new Player(n.Key, rand.Next(20, 30), i, new Deck()
                    {
                        name = "defaultDeck"
                    });
                }));

            players.ForEach(p => p.deck.cards = new List<Card>()
            {
                new Card() { name = "attack", timeCost = 2, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 5 }} },
                new Card() { name = "attack", timeCost = 2, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 5 }} },
                new Card() { name = "attack", timeCost = 2, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 5 }} },
                new Card() { name = "attack", timeCost = 2, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 5 }} },
                new Card() { name = "attack", timeCost = 2, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 5 }} },
                new Card() { name = "attack", timeCost = 2, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 5 }} },
                new Card() { name = "heal", timeCost = 3, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 3 }} },
                new Card() { name = "heal", timeCost = 3, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 3 }} },
                new Card() { name = "heal", timeCost = 3, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 3 }} },
                new Card() { name = "heal", timeCost = 3, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 3 }} },
                new Card() { name = "draw", timeCost = 4, effects = new List<ICardEffect>() { new CardEffectDraw(p, null) { value = 3 }} },
                new Card() { name = "draw", timeCost = 4, effects = new List<ICardEffect>() { new CardEffectDraw(p, null) { value = 3 }} },
                //new Card() { name = "small attack", timeCost = 1, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 3 }} },
                //new Card() { name = "small attack", timeCost = 1, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 3 }} },
                //new Card() { name = "small attack", timeCost = 1, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 3 }} },
                //new Card() { name = "small attack", timeCost = 1, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 3 }} },
                //new Card() { name = "attack", timeCost = 2, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 5 }} },
                //new Card() { name = "attack", timeCost = 2, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 5 }} },
                //new Card() { name = "small heal", timeCost = 2, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 1 }} },
                //new Card() { name = "small heal", timeCost = 2, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 1 }} },
                //new Card() { name = "heal", timeCost = 3, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 3 }} },
                //new Card() { name = "heal", timeCost = 3, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 3 }} },
                //new Card() { name = "heal", timeCost = 3, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 3 }} },
                //new Card() { name = "heal", timeCost = 3, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 3 }} },
                //new Card() { name = "heal", timeCost = 3, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 3 }} },
                //new Card() { name = "draw", timeCost = 4, effects = new List<ICardEffect>() { new CardEffectDraw(p, null) { value = 3 }} },
                //new Card() { name = "draw", timeCost = 4, effects = new List<ICardEffect>() { new CardEffectDraw(p, null) { value = 3 }} },
                //new Card() { name = "draw", timeCost = 4, effects = new List<ICardEffect>() { new CardEffectDraw(p, null) { value = 3 }} },
                //new Card() { name = "draw", timeCost = 4, effects = new List<ICardEffect>() { new CardEffectDraw(p, null) { value = 3 }} },
                //new Card() { name = "big attack", timeCost = 5, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 10 }} },
                //new Card() { name = "big attack", timeCost = 5, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 10 }} },
                //new Card() { name = "grosse attaque", timeCost = 5, effects = new List<ICardEffect>() { new CardEffectDamage(p, null) { value = 10 }} },
                //new Card() { name = "super heal", timeCost = 10, effects = new List<ICardEffect>() { new CardEffectHeal(p, null) { value = 10 }} },
            });

            players.ForEach(p =>
            {
                p.deck.cards.ForEach(c => c.effects.ForEach(e =>
                {
                    CardEffect effect = e as CardEffect;
                    effect.card = c;
                }));
                p.drawPile.Push(p.deck.cards.ToArray());
            });

            List <Monster> monsters = new List<Monster>()
            {
                new Monster("monster", 20, 0, 15.0)
            };

            battle = new Battle(this, players, monsters);

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                battle.PlayersTurnStart();
            });
        }

        internal void HandleBattleEnd()
        {
            battle = null;
        }

        public PD_BattleState GetBattleState(string playerName)
        {
            Player mainPlayer = battle.players.Find(p => p.name == playerName);

            var lol = new PD_BattleState()
            {
                monsters = battle.monsters.Select(m =>
                {
                    return new PD_BattleStateMonster()
                    {
                        location = m.location,
                        name = m.name,
                        life = m.life
                    };
                }).ToList(),

                players = battle.players.Select(p =>
                {
                    return new PD_BattleStatePlayer()
                    {
                        location = p.location,
                        name = p.name,
                        life = p.life,
                        handCount = p.hand.Count,
                        drawPileCount = p.drawPile.Count,
                        discardPileCount = p.discardPile.Count
                    };
                }).Where(p => p.name != playerName).ToList(),

                mainPlayer = new PD_BattleStateMainPlayer()
                {
                    location = mainPlayer.location,
                    life = mainPlayer.life,
                    hand = mainPlayer.hand.Cards.Select(c => c.name).ToList(),
                    drawPileCount = mainPlayer.drawPile.Count,
                    discardPileCount = mainPlayer.discardPile.Count
                }
            };
            return lol;
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