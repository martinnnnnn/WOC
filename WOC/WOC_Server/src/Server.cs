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
    public class WOCServer
    {
        public IPAddress IP;
        public int Port;

        private TcpListener listener;
        private CancellationTokenSource tokenSource;
        private bool listening;

        private CancellationToken token;
        private SynchronizedCollection<Session> sessions = new SynchronizedCollection<Session>();

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

                        Session session = new Session();
                        session.Connect(client);
                        sessions.Add(session);
                        LOG.Print("[SERVER] Client connected. {0} clients connected", sessions.Count);
                        session.OnMessageReceived += IncomingHandling;
                        session.OnDisconnect += () =>
                        {
                            sessions.Remove(session);
                            LOG.Print("[SERVER] Client closed. {0} clients still connected", sessions.Count);

                        };
                    }, token);
                }
            }
            finally
            {
                LOG.Print("[SERVER] Closing server.");
                listener.Stop();
                foreach (Session s in sessions)
                {
                    s.Close();
                }
                listening = false;
                LOG.Print("[SERVER] Server closed.");
            }
        }

        public void Close()
        {
            if (listening)
            {
                LOG.Print("Closing server");
                tokenSource?.Cancel();
                //listener.Stop();
                Session closer = new Session();
                closer.Connect("127.0.0.1", 54001);
                
            }
            else
            {
                LOG.Print("[SERVER] already closed.");
            }
        }
        void IncomingHandling(IPacketData data)
        {
            LOG.Print("[SERVER] received a packet.");

            switch (data)
            {
                case PD_Chat chat:
                    var tasks = new List<Task>();
                    try
                    {
                        foreach (Session s in sessions)
                        {
                            tasks.Add(Task.Run(async () => { await s.SendAsync(chat); }));
                        }
                        Task.WaitAll(tasks.ToArray(), 10000);
                    }
                    catch (Exception)
                    {
                        LOG.Print("[SERVER] Failed to broadcast message.");
                    }
                    break;

            }
        }
        public async Task Broadcast(string msg)
        {
            var tasks = sessions.Select(session => session.SendAsync(msg));
            await Task.WhenAll(tasks);
        }
        public async Task Broadcast(IPacketData data)
        {
            var tasks = sessions.Select(session => session.SendAsync(data));
            await Task.WhenAll(tasks);
        }

    }
}





//namespace WOC_Server
//{

//    public class Server
//    {
//        object threadLock = new object();
//        List<Task> connections = new List<Task>();
//        List<BattleInstance> battles = new List<BattleInstance>();

//        public List<ServerSideSession> sessions = new List<ServerSideSession>();
//        TcpListener listener;

//        public void TryConnect()
//        {

//        }



//        public async Task Broadcast(List<Session> sessions, string message)
//        {
//            var tasks = sessions.Select(session => session.SendAsync(message));
//            await Task.WhenAll(tasks);
//        }


//        public void HandleIncoming(Session sender, IPacketData packet)
//        {
//            string errMessage = "";
//            switch(packet)
//            {
//                case PD_Chat data:
//                    Broadcast(PacketData.ToJson(data)).Wait();
//                    break;
//                case PD_Create<PD_BattleAction> data:
//                    CreateBattle(sender, data);
//                    break;
//                case PD_BattleAction data:
//                    var battle = battles.FirstOrDefault(b => b.info.name == data.battleName);
//                    if (battle != null)
//                    {
//                        battles.Find(b => b.info.name == data.battleName)?.HandleIncoming(sender, data);
//                    }
//                    else
//                    {
//                        sender.SendValidation(data.id, "battle_name_not_found").Wait();
//                    }
//                    break;
//            }

//            sender.SendValidation(packet.id, errMessage).Wait();
//        }

//        void CreateBattle(Session sender, PD_Create<PD_BattleAction> data)
//        {
//            string errMessage = string.Empty;
//            if (battles.Find(b => b.info.name == data.toCreate.battleName) != null)
//            {
//                errMessage = "battle_name_already_used";
//            }
//            else
//            {
//                battles.Add(new BattleInstance());
//            }

//            sender.SendValidation(data.id, errMessage).Wait();
//        }

//        public async Task StartListenerAsync()
//        {
//            listener = TcpListener.Create(8000);
//            listener.Start();
//            while (true)
//            {
//                var tcpClient = await listener.AcceptTcpClientAsync();
//                Console.WriteLine("[Server] Client has connected");
//                var task = StartHandleConnectionAsync(tcpClient);
//                if (task.IsFaulted)
//                {
//                    task.Wait();
//                }
//            }
//        }
        
//        public bool FindSession(Account account, out Session session)
//        {
//            foreach (var sess in sessions)
//            {
//                if (sess.account == account)
//                {
//                    session = sess;
//                    return true;
//                }
//            }
//            session = null;
//            return false;
//        }

//        public bool FindSession(string accountName, out Session session)
//        {
//            foreach (var sess in sessions)
//            {
//                if (sess.account.name == accountName)
//                {
//                    session = sess;
//                    return true;
//                }
//            }
//            session = null;
//            return false;
//        }

//        public void Close()
//        {
//            listener.Stop();
//            //connection.Close();
//        }

//        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
//        {
//            ServerSideSession session = new ServerSideSession(tcpClient, this);
//            var sessionTask = Task.Run(() => session.ListenAsync());
//            //var sessionTask = session.StartAsync();
//            lock (threadLock)
//            {
//                connections.Add(sessionTask);
//                sessions.Add(session);
//            }

//            Console.WriteLine("Connect : currently {0} users connected on {1} tasks", sessions.Count, connections.Count);

//            try
//            {
//                await sessionTask;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.ToString());
//            }
//            finally
//            {
//                lock (threadLock)
//                {
//                    connections.Remove(sessionTask);
//                    sessions.Remove(session);
//                }
//                Console.WriteLine("Disconnect : currently {0} users connected on {1} tasks", sessions.Count, connections.Count);
//            }
//        }
//    }
//}
