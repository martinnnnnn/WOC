using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WOC_Core;


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
        private SynchronizedCollection<ServerSession> sessions = new SynchronizedCollection<ServerSession>();

        public  Battle battle;

        public async Task StartAsync(IPAddress ip, int port)
        {
            if (listening)
            {
                Close();
            }

            IP = ip;
            Port = port;
            listener = new TcpListener(IP, Port);
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
            token = tokenSource.Token;
            listener.Start();
            listening = true;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Run(async () =>
                    {
                        var tcpClientTask = listener.AcceptTcpClientAsync();
                        TcpClient client = await tcpClientTask;

                        ServerSession session = new ServerSession(this, battle);
                        session.Connect(client);
                        session.OnDisconnect += () =>
                        {
                            sessions.Remove(session);
                            LOG.Print("[SERVER] Client closed. {0} clients still connected", sessions.Count);
                        };
                        sessions.Add(session);
                        LOG.Print("[SERVER] Client connected. {0} clients connected", sessions.Count);
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
                LOG.Print("[SERVER] Closing server.");
                listener.Stop();
                foreach (Session s in sessions)
                {
                    s?.Close();
                }
                sessions.Clear();
                listening = false;
                LOG.Print("[SERVER] Server closed.");
            }
            else
            {
                LOG.Print("[SERVER] already closed.");
            }
        }


        public async Task Broadcast(string msg, Session toIgnore = null)
        {
            LOG.Print("broadcasting");
            List<Task> tasks = new List<Task>();

            foreach (Session session in sessions)
            {
                if (toIgnore == null || session != toIgnore)
                {
                    tasks.Add(session.SendAsync(msg));
                }
            }
            await Task.WhenAll(tasks);
        }
        public async Task Broadcast(IPacketData data, Session toIgnore = null)
        {
            await Broadcast(Serialization.ToJson(data), toIgnore);
        }

        public void Init()
        {
            LOG.Print("[SERVER] Battle initialization...");
            battle = new Battle();

            Initiative.Max = 50;

            // CARDS
            List<Card> cardsMap = new List<Card>()
            {
                // name | mana cost | exhaust | effects list
                new Card("smol_dmg", 1, false, new List<CardEffect>
                {
                    new CardEffectDamage(1)
                }),
                new Card("hek", 2, false, new List<CardEffect>
                {
                    new CardEffectHeal(2)
                }),
                new Card("big_dmg", 3, false, new List<CardEffect>
                {
                    new CardEffectDamage(4)
                })
            };
            LOG.Print("[SERVER] Adding card");
            cardsMap.ForEach(c => battle.Add(c));
        }

        public void RunBattle()
        {
            battle.Init();
            battle.OnBattleEnd += BattleOver;

            LOG.Print("> Battle starting !");
            while (!isOver)
            {
                Actor current = battle.NextActor();
                switch (current)
                {
                    case PlayerActor player:
                        ServerSession session = sessions.First(p => p.actor == player);
                        StartTurn(session);
                        break;
                    case PNJActor pnj:
                        LOG.Print("> {0} playing !", pnj.Name);
                        break;
                }
            }
        }

        void StartTurn(ServerSession playerSession)
        {
            playerSession.actor.StartTurn();
        }


        bool isOver = false;
        void BattleOver()
        {
            isOver = true;
        }

    }
}