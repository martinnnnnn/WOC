//using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WOC_Network;

namespace WOC_Server
{

    public class Server
    {
        object threadLock = new object();
        List<Task> connections = new List<Task>();
        List<BattleInstance> battles = new List<BattleInstance>();

        public List<ServerSideSession> sessions = new List<ServerSideSession>();
        TcpListener listener;
        //public MySqlConnection connection;

        //public void SqlConnection()
        //{
        //    string connectionString = "Data Source=localhost;Initial Catalog=woc;User ID=martin;Password=Jenesaispas:1204";
        //    connection = new MySqlConnection(connectionString);
        //    connection.Open();
        //    MySqlCommand sqlcmd = connection.CreateCommand();
        //    sqlcmd.CommandText = "INSERT INTO decks (name, account_id) SELECT @deck_name, id FROM accounts WHERE name = @account_name";
        //    sqlcmd.Parameters.AddWithValue("@account_name", "martin");
        //    sqlcmd.Parameters.AddWithValue("@deck_name", "my_healer");
        //    sqlcmd.ExecuteNonQuery();
        //}

        public void TryConnect()
        {

        }

        public async Task Broadcast(string message)
        {
            var tasks = sessions.Select(session => session.SendAsync(message));
            await Task.WhenAll(tasks);
        }

        public async Task Broadcast(List<Session> sessions, string message)
        {
            var tasks = sessions.Select(session => session.SendAsync(message));
            await Task.WhenAll(tasks);
        }


        public void HandleIncoming(Session sender, IPacketData packet)
        {
            switch(packet)
            {
                case PD_Chat data:
                    Broadcast(PacketData.ToJson(data)).Wait();
                    break;
                case PD_Create<BattleInfo> data:
                    CreateBattle(sender, data);
                    break;
                case PD_BattleAction data:
                    var battle = battles.FirstOrDefault(b => b.info.name == data.battleName);
                    if (battle != null)
                    {
                        battles.Find(b => b.info.name == data.battleName)?.HandleIncoming(sender, data);
                    }
                    else
                    {
                        sender.SendValidation(data.id, "battle_name_not_found").Wait();
                    }
                    break;
            }

            sender.SendValidation(data.id, errMessage).Wait();
        }

        void CreateBattle(Session sender, PD_Create<BattleInfo> data)
        {
            string errMessage = string.Empty;
            if (battles.Find(b => b.info.name == data.toCreate.name) != null)
            {
                errMessage = "battle_name_already_used";
            }
            else
            {
                battles.Add(new BattleInstance() { info = data.toCreate });
            }

            sender.SendValidation(data.id, errMessage).Wait();
        }

        public async Task StartListenerAsync()
        {
            listener = TcpListener.Create(8000);
            listener.Start();
            while (true)
            {
                var tcpClient = await listener.AcceptTcpClientAsync();
                Console.WriteLine("[Server] Client has connected");
                var task = StartHandleConnectionAsync(tcpClient);
                if (task.IsFaulted)
                {
                    task.Wait();
                }
            }
        }
        
        public bool FindSession(Account account, out Session session)
        {
            foreach (var sess in sessions)
            {
                if (sess.account == account)
                {
                    session = sess;
                    return true;
                }
            }
            session = null;
            return false;
        }

        public bool FindSession(string accountName, out Session session)
        {
            foreach (var sess in sessions)
            {
                if (sess.account.name == accountName)
                {
                    session = sess;
                    return true;
                }
            }
            session = null;
            return false;
        }

        public void Close()
        {
            listener.Stop();
            //connection.Close();
        }

        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
        {
            ServerSideSession session = new ServerSideSession(tcpClient, this);
            var sessionTask = Task.Run(() => session.StartAsync());
            //var sessionTask = session.StartAsync();
            lock (threadLock)
            {
                connections.Add(sessionTask);
                sessions.Add(session);
            }

            Console.WriteLine("Connect : currently {0} users connected on {1} tasks", sessions.Count, connections.Count);

            try
            {
                await sessionTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                lock (threadLock)
                {
                    connections.Remove(sessionTask);
                    sessions.Remove(session);
                }
                Console.WriteLine("Disconnect : currently {0} users connected on {1} tasks", sessions.Count, connections.Count);
            }
        }
    }
}
