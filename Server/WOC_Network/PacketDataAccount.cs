using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Network
{
    public class PacketDataAccountCreate : IPacketData
    {
        public string name;
        public string password;
    }

    public class PacketDataAccountConnect : IPacketData
    {
        public string name;
        public string password;
    }

    public class PacketDataAccountDisconnect : IPacketData
    {
        public string name;
        public string password;
    }

    public class PacketDataAccountList : IPacketData
    {
    }
}
