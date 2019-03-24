using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WOC_Network;
using UnityEngine;

namespace WOC
{
    public class ClientSideSession : Session
    {
        public Network network;

        public ClientSideSession(TcpClient tcpClient, Network net) : base(tcpClient)
        {
            network = net;
        }

        protected override void HandleIncoming(string jmessage)
        {
            network.HandleIncoming(jmessage);
        }
    }
}
