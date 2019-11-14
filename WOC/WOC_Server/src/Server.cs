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