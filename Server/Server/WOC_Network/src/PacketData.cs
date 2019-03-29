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

        public PD_Validate(Guid valId, string message = "")
        {
            validationId = valId;
            isValid = string.IsNullOrEmpty(message);
            errorMessage = message;
        }

        public PD_Validate(bool valid, string message = "")
        {
            isValid = valid;
            errorMessage = message;
        }
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
        public string password;
    }

    public class PD_AccountDisconnect : IPacketData
    {
        public string name;
    }
    public class PD_BattleAction : IPacketData
    {
        public string battleName;
    }

    public class PD_BattleCardPlayed : PD_BattleAction
    {
        public string cardName;
        public string owner;
        public string target;
    }

    public class PD_BattleMonsterAction : PD_BattleAction
    {
        public string monsterName;
        public string actionName;
        public string target;
    }

    public class PD_BattleStart : PD_BattleAction {}
    
    public class PD_BattleRegister : PD_BattleAction
    {
        public string accountName;
    }
    public class PD_BattleDisconnect : PD_BattleAction
    {
        public string accountName;
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
