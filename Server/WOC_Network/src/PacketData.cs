using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WOC_Network
{
    public abstract class IPacketData { public Guid id = Guid.NewGuid(); }

    public class PD_Validate : IPacketData
    {
        public Guid validationId;
        public bool isValid;
        public string errorMessage;
    }

    public class PD_InfoRequest : IPacketData
    {
        public string infoType;
    }

    public class PD_Info<T> : IPacketData
    {
        public T info;
    }

    public class PD_Chat : IPacketData
    {
        public string senderName;
        public string message;
    }

    public class PD_Create<T> : IPacketData
    {
        public T toCreate;
    }

    public class PD_AccountConnect : IPacketData
    {
        public string name;
    }

    public class PD_AccountDisconnect : IPacketData
    {
        public string name;
    }

    public class AccountList
    {
        public List<string> names;
    }

    public class PacketData
    {
        static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static string ToJson(IPacketData packet, bool indent = false)
        {
            return JsonConvert.SerializeObject(packet, settings);
        }

        public static IPacketData FromJson(string jpackage)
        {
            IPacketData data;
            try
            {
                data = JsonConvert.DeserializeObject<IPacketData>(jpackage, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                data = null;
            }
            return data;
        }
    }
}
