using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Network
{
    public class Session
    {
        public Connection connection = null;
        public Account account = null;

        public Session(TcpClient tcpClient)
        {
            connection = new Connection(tcpClient);
            connection.HandleIncomingMessage = HandleIncoming;
        }

        public async Task SendAsync(string message)
        {
            await connection.Send(message);
        }

        public async Task StartAsync()
        {
            await Task.Yield();
            await connection.HandleConnectionAsync();
        }

        protected virtual void HandleIncoming(string message)
        {
            Console.WriteLine("I'm don't know how to handle {0}", message);
        }
    }



}
