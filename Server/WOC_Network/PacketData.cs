using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WOC_Network
{

    public class PacketData
    {
        public string type;
        public Dictionary<string, object> data = new Dictionary<string, object>();

        public static string ToJson(PacketData packet, bool indent = false)
        {
            return JsonConvert.SerializeObject(packet, Formatting.Indented);
        }
        public static PacketData FromJson(string jpackage)
        {
            return JsonConvert.DeserializeObject<PacketData>(jpackage);
        }
    }

}
