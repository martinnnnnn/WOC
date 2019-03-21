using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WOC
{
    public class ClientSideSession : WOC_Network.Session
    {
        public ClientSideSession(TcpClient tcpClient) : base(tcpClient)
        {
        }
    }
}
