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
        public ClientSideSession(TcpClient tcpClient, Network net) : base(tcpClient)
        {
            network = net;
        }

        public Network network;
        //public Action<PD_Validate> HandleValidate;
        //public Action<PD_Info<Account>> HandleAccountInfo;
        //public Action<PD_Info<AccountList>> HandleAccountList;
        //public Action<PD_Chat> HandleChatMessage;

        void hello(PD_Validate val)
        {

        }

        protected override void HandleIncoming(string message)
        {
            network.HandleIncoming(message);
            //try
            //{
            //    IPacketData packet = PacketData.FromJson(message);

            //    if (packet != null)
            //    {
            //        switch (packet)
            //        {
            //            case PD_Validate data:
            //                HandleValidate?.Invoke(data);
            //                break;
            //            case PD_Info<Account> data:
            //                HandleAccountInfo?.Invoke(data);
            //                break;
            //            case PD_Info<AccountList> data:
            //                Debug.Log("received account list" + HandleAccountList != null);
            //                HandleAccountList(data);
            //                break;
            //            case PD_Chat data:
            //                Debug.Log("received chat message");
            //                HandleChatMessage?.Invoke(data);
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine("Unknow JSON message : " + message);
            //    }
            //}
            //catch (Exception)
            //{
            //    Console.WriteLine("Error while parsing JSON message : " + message);
            //}
        }
    }
}
