using System;
using System.Collections.Generic;

namespace WOC_Network
{
    public class Packet
    {
        public string type;
        public Dictionary<string, object> data = new Dictionary<string, object>();
    }
}
