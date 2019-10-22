using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WOC_Core;
using System.Collections.Concurrent;

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





        public List<BattleRoom> battleRooms = new List<BattleRoom>();

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
                            battleRooms.ForEach(r => r.Remove(session));
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
            return (battleRooms.Find(r => r.Name == roomName) != null);
        }

        public bool CreateBattleRoom(string roomName)
        {
            if (battleRooms.Find(r => r.Name == roomName) == null)
            {
                Random random = new Random();
                battleRooms.Add(new BattleRoom(roomName, this, random.Next()));
                return true;
            }
            return false;
        }
        public bool MoveToBattleRoom(string roomName, ServerSession session)
        {
            BattleRoom room = battleRooms.Find(r => r.Name == roomName);
            if (room != null)
            {
                sessions.Remove(session);
                room.Add(session);
                return true;
            }
            return false;
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
                battleRooms.Clear();
                listening = false;
                LOG.Print("[SERVER] Server closed.");
            }
            else
            {
                LOG.Print("[SERVER] already closed.");
            }
        }


        public async Task Broadcast(string msg, Session toIgnore = null, bool toAll = false)
        {
            List<Task> tasks = new List<Task>();
            
            if (toAll)
            {
                battleRooms.ForEach(r => tasks.Add(r.Broadcast(msg, toIgnore)));
            }

            foreach (Session session in sessions)
            {
                if (toIgnore == null || session != toIgnore)
                {
                    tasks.Add(session.SendAsync(msg));
                }
            }

            await Task.WhenAll(tasks);
        }

        public async Task Broadcast(IPacketData data, Session toIgnore = null, bool toAll = false)
        {
            await Broadcast(Serialization.ToJson(data), toIgnore, toAll);
        }
    }
}