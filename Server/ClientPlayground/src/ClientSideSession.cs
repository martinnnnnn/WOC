using Newtonsoft.Json;
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
                        case PD_Validate data:
                            break;
                        case PD_Info<Account> data:
                            Console.WriteLine("rece");
                            Account acc = data.info;
                            break;

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

        //public async Task<PD_Validate> GetValidationAsync()
        //{
        //    session
        //}
    }
}
