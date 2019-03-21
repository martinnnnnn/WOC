using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WOC_Network
{
    public interface IPacketData { }

    public class PD_AccountCreate : IPacketData
    {
        public string name;
    }

    public class PD_AccountConnect : IPacketData
    {
        public string name;
    }

    public class PD_AccountDisconnect : IPacketData
    {
        public string name;
    }

    public class PD_AccountList : IPacketData
    {
    }

    public class PD_AccountInfo : IPacketData
    {
        public Account account;
    }

    public class PD_CharacterCreate : IPacketData
    {
        public Character character;
    }

    public class PD_DeckCreate : IPacketData
    {
        public Deck deck;
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
                data = new PD_AccountList();
            }
            return data;
        }
    }
}
