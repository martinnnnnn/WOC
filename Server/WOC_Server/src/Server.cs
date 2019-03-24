﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WOC_Network;

namespace WOC_Server
{

    public class Server
    {
        object _lock = new object();
        List<Task> _connections = new List<Task>();

        public List<ServerSideSession> sessions = new List<ServerSideSession>();
        TcpListener listener;
        public MySqlConnection connection;

        public void SqlConnection()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=woc;User ID=martin;Password=Jenesaispas:1204";
            connection = new MySqlConnection(connectionString);
            connection.Open();
            MySqlCommand sqlcmd = connection.CreateCommand();
            sqlcmd.CommandText = "INSERT INTO decks (name, account_id) SELECT @deck_name, id FROM accounts WHERE name = @account_name";
            sqlcmd.Parameters.AddWithValue("@account_name", "martin");
            sqlcmd.Parameters.AddWithValue("@deck_name", "my_healer");
            sqlcmd.ExecuteNonQuery();
        }

        public void TryConnect()
        {

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

        public void Close()
        {
            listener.Stop();
            connection.Close();
        }

        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
        {
            ServerSideSession session = new ServerSideSession(tcpClient, this);
            var sessionTask = session.StartAsync();

            lock (_lock)
            {
                _connections.Add(sessionTask);
                sessions.Add(session);
            }

            Console.WriteLine("Connect : currently {0} users connected on {1} tasks", sessions.Count, _connections.Count);

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
                lock (_lock)
                {
                    _connections.Remove(sessionTask);
                    sessions.Remove(session);
                }
                Console.WriteLine("Disconnect : currently {0} users connected on {1} tasks", sessions.Count, _connections.Count);
            }
        }
    }
}
