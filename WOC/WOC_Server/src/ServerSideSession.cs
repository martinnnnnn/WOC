using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WOC_Core;

namespace WOC_Server
{
    public class ServerSession : Session
    {
        TCPServer server;

        public ServerSession(TCPServer s)
        {
            server = s;
        }
        public override void HandleIncomingMessage(IPacketData data)
        {
            LOG.Print("[SERVER] received a packet. {0}", data);
            base.HandleIncomingMessage(data);

            switch (data)
            {
                case PD_Chat chat:
                    var tasks = new List<Task>();
                    try
                    {
                        server.Broadcast(chat).Wait();
                    }
                    catch (Exception)
                    {
                        LOG.Print("[SERVER] Failed to broadcast message.");
                    }
                    break;
            }
        }
    }
}
