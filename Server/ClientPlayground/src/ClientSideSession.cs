using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WOC_Network;

namespace WOC
{
    public class ClientSideSession : Session
    {
        public ClientSideSession(TcpClient tcpClient) : base(tcpClient)
        {
        }

        protected override void HandleIncoming(string message)
        {
            Console.WriteLine("I'm handling {0}", message);
            try
            {
                IPacketData packet = PacketData.FromJson(message);

                if (packet != null)
                {
                    switch (packet)
                    {
                        
                    }
                }
                else
                {
                    Console.WriteLine("Unknow JSON message : " + message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error while parsing JSON message : " + message);
            }
        }
    }
}
