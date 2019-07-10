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
            LOG.Print("[SERVER] Starting to listen on {0}:{1}", ip, port);
            if (listening)
            {
                LOG.Print("[SERVER] Already started, trying to close first...");
                Close();
            }

            IP = ip;
            Port = port;
            listener = new TcpListener(IP, Port);
            tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new CancellationToken());
            token = tokenSource.Token;
            listener.Start();
            listening = true;
            LOG.Print("[SERVER] Server operational.");

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
            LOG.Print("Broadcasting {0}", msg);
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
            LOG.Print("[SERVER] Welcome to the WOC Server ! ");
            LOG.Print("[SERVER] Battle initialization...");
            battle = new Battle();
            battle.OnBattleEnd += BattleOver;
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
                    new CardEffectDamage(10)
                })
            };
            LOG.Print("[SERVER] Adding cards");
            cardsMap.ForEach(c => battle.Add(c));

            //PNJS
            List<Actor> actors = new List<Actor>()
            {
                // battle | character | name | first init
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre1", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre2", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.CHAMAN, 15), "monstre3", 5)
            };
            LOG.Print("[SERVER] Adding PNJs");
            actors.ForEach(a => battle.Add(a));
        }

        void BattleOver()
        {
            LOG.Print("Battle over !!! ");
            LOG.Print("...");
            LOG.Print("Resetting battle !");
            Init();
        }
    }
}