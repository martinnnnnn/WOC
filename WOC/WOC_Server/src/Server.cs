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
        public List<Card> cards = new List<Card>();

        public TCPServer()
        {
            cards = new List<Card>()
            {
                // name | mana cost | exhaust | effects list
                new Card("smol_dmg", 1, false, new List<CardEffect>
                {
                    new CardEffectDamage(5)
                }),
                new Card("hek", 2, false, new List<CardEffect>
                {
                    new CardEffectHeal(2)
                }),
                new Card("big_dmg", 3, false, new List<CardEffect>
                {
                    new CardEffectDamage(10)
                }),
                new Card("smol_dmg2", 1, false, new List<CardEffect>
                {
                    new CardEffectDamage(5)
                }),
                new Card("hek2", 2, false, new List<CardEffect>
                {
                    new CardEffectHeal(2)
                }),
                new Card("big_dmg2", 3, false, new List<CardEffect>
                {
                    new CardEffectDamage(10)
                }),
                    new Card("smol_dmg3", 1, false, new List<CardEffect>
                {
                    new CardEffectDamage(5)
                }),
                new Card("hek3", 2, false, new List<CardEffect>
                {
                    new CardEffectHeal(2)
                }),
                new Card("big_dmg3", 3, false, new List<CardEffect>
                {
                    new CardEffectDamage(10)
                }),
                new Card("smol_dmg4", 1, false, new List<CardEffect>
                {
                    new CardEffectDamage(5)
                }),
                new Card("hek4", 2, false, new List<CardEffect>
                {
                    new CardEffectHeal(2)
                }),
                new Card("big_dmg4", 3, false, new List<CardEffect>
                {
                    new CardEffectDamage(10)
                }),
                new Card("smol_dmg5", 1, false, new List<CardEffect>
                {
                    new CardEffectDamage(5)
                }),
                new Card("hek5", 2, false, new List<CardEffect>
                {
                    new CardEffectHeal(2)
                }),
                new Card("big_dmg5", 3, false, new List<CardEffect>
                {
                    new CardEffectDamage(10)
                }),
                new Card("smol_dmg6", 1, false, new List<CardEffect>
                {
                    new CardEffectDamage(5)
                }),
                new Card("hek6", 2, false, new List<CardEffect>
                {
                    new CardEffectHeal(2)
                }),
                new Card("big_dmg6", 3, false, new List<CardEffect>
                {
                    new CardEffectDamage(10)
                }),
                new Card("smol_dmg7", 1, false, new List<CardEffect>
                {
                    new CardEffectDamage(5)
                }),
                new Card("hek7", 2, false, new List<CardEffect>
                {
                    new CardEffectHeal(2)
                }),
                new Card("big_dmg7", 3, false, new List<CardEffect>
                {
                    new CardEffectDamage(10)
                }),
            };
        }


        public List<Room> rooms = new List<Room>();

        public async Task StartAsync(IPAddress ip, int port)
        {
            LOG.Print("[SERVER] Welcome to the WOC Server ! ");
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

                        ServerSession session = new ServerSession(this);
                        session.Connect(client);
                        //Broadcast(new PD_SessionConnect { name = session.Name }, null, true).Wait();
                        session.OnDisconnect += () =>
                        {
                            sessions.Remove(session);
                            rooms.ForEach(r => r.Remove(session));
                            //Broadcast(new PD_SessionDisconnect { name = session.Name }, null, true).Wait();
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

        public bool Exists(string roomName)
        {
            return (rooms.Find(r => r.Name == roomName) != null);
        }

        public Room CreateRoom(string roomName)
        {
            Debug.Assert(rooms.Find(r => r.Name == roomName) == null);

            Random random = new Random();
            Room room = new Room(roomName, this, random.Next());
            rooms.Add(room);
            return room;
        }
        public void MoveToRoom(Room room, ServerSession session)
        {
            Debug.Assert(rooms.Find(r => r == room) != null);
            sessions.Remove(session);
            room.Add(session);
        }

        public void LeaveRoom(ServerSession session)
        {

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
                rooms.Clear();
                listening = false;
                LOG.Print("[SERVER] Server closed.");
            }
            else
            {
                LOG.Print("[SERVER] already closed.");
            }
        }


        public async Task Broadcast(IPacketData data, Session toIgnore = null, bool toAll = false)
        {
            List<Task> tasks = new List<Task>();
            
            if (toAll)
            {
                rooms.ForEach(r => tasks.Add(r.Broadcast(data, toIgnore)));
            }

            foreach (Session session in sessions)
            {
                if (toIgnore == null || session != toIgnore)
                {
                    tasks.Add(session.SendAsync(data));
                }
            }

            await Task.WhenAll(tasks);
        }

        public async Task Broadcast(IPacketData data, IEnumerable<ServerSession> sessions)
        {
            List<Task> tasks = new List<Task>();

            foreach (Session session in sessions)
            {
                 tasks.Add(session.SendAsync(data));
            }

            await Task.WhenAll(tasks);
        }
    }
}