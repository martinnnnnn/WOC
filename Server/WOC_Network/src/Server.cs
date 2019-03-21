using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Network
{

    public class Server
    {
        object _lock = new object();
        List<Task> _connections = new List<Task>();

        public List<Session> sessions = new List<Session>();
        TcpListener listener;

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
        }

        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
        {
            Session session = new ServerSideSession(tcpClient, this);
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
