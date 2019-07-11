using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WOC_Core;

namespace WOC_Core
{
    public abstract class IPacketData { public Guid id = Guid.NewGuid(); }

    public class PD_Validation : IPacketData
    {
        public Guid validationId;
        public bool isValid;
        public string errorMessage;

        public PD_Validation()
        {
        }

        public PD_Validation(Guid valId, string message = "")
        {
            validationId = valId;
            isValid = string.IsNullOrEmpty(message);
            errorMessage = message;
        }

        public PD_Validation(Guid valId, bool valid, string message = "")
        {
            isValid = valid;
            errorMessage = message;
        }
    }

    public class PD_SessionShutdown : IPacketData { }

    public class PD_NameModify : IPacketData
    {
        public string name;
    }
    public class PD_Chat : IPacketData
    {
        public string senderName;
        public string message;
    }

    public class PD_BattleCreate : IPacketData
    {
        public string name;
    }
    public class PD_BattleJoin : IPacketData
    {
        public string name;
    }
    public class PD_BattleLeave : IPacketData {}
    public class PD_BattleStart : IPacketData {}

    public class PD_BattleList : IPacketData
    {
        public List<string> rooms;
    }
    
    public class PD_PlayerList : IPacketData
    {
        public string roomName;
        public List<string> players;
    }
    public class PD_TurnEnd : IPacketData { }

    public class PD_PlayerAdd : IPacketData
    {
        public string name;
        public Character.Race charaRace;
        public Character.Category charaCategory;
        public int          charaLife;
        public string       charaName;
        public int          handStartCount;
        public int          handMaxCount;
        public List<string> cardsName;
        public int          aggroIncrement;
        public int          manaMax;
    }

    public class PD_CardPlayed : IPacketData
    {
        public string ownerName;
        public string targetName;
        public int cardIndex;
        public string cardName;
    }
}
