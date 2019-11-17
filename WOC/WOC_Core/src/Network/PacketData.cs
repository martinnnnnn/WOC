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

    public class PD_Discovery : IPacketData { }


    /*
 * API :
 *      
 *      
 *      ACCOUNT
 *      - make account
 *      - modify account
 *      - delete account
 *      - connect account
 *      - disconnect account
 *      - add character
 *      - remove character
 *      - add account (as friend)
 *      - remove account (as friend)
 *      - add character
 *      - modify character
 *      - delete character
 *      - set default character

 *      SERVER
 *      - send message (all / private / room / friends)
 *      - make room
 *      - rename room
 *      - join room
 *      - list room
 *      - list players (in server)
 *      - list players (in room)
 *      
 *      ROOM
 *      - add PNJ
 *      - battle init
 *      - battle start
 *      - battle end
 *      
 *      BATTLE
 *      - state
 *      - card played
 *      - turn end
 * */


    /// ///////////////////////////////////////////////////////////////////////////////////////
    ///                                         ACCOUNT
    /// ///////////////////////////////////////////////////////////////////////////////////////
    public class PD_AccountMake : IPacketData
    {
        public string email;
        public string password;
        public string name;
    }

    public class PD_AccountModify : IPacketData
    {
        public string oldEmail;
        public string oldPassword;
        public string oldName;

        public string newEmail;
        public string newPassword;
        public string newName;
    }

    public class PD_AccountNameModify : IPacketData
    {
        public string oldName;
        public string newName;
    }

    public class PD_AccountDelete : IPacketData
    {
        public string email;
        public string password;
    }

    public class PD_AccountDeleted : IPacketData
    {
        public string name;
    }

    public class PD_AccountConnect : IPacketData
    {
        public string email;
        public string password;
    }

    public class PD_AccountConnected : IPacketData
    {
        public string name;
    }

    public class PD_AccountDisconnect : IPacketData
    {
        public string email;
    }

    public class PD_AccountDisconnected : IPacketData
    {
        public string name;
    }

    public class PD_AccountAddFriend : IPacketData
    {
        public string name;
    }

    public class PD_AccountRemoveFriend : IPacketData
    {
        public string name;
    }

    public class PD_AccountNewDeck : IPacketData
    {
        public string name;
    }
    public class PD_AccountAddCard : IPacketData
    {
        public string deckName;
        public string cardName;
    }

    public class PD_AccountRenameDeck : IPacketData
    {
        public string oldName;
        public string newName;
    }

    public class PD_AccountDeleteDeck : IPacketData
    {
        public string name;
    }

    public class PD_AccountSetCurrentDeck : IPacketData
    {
        public string name;
    }


    /// ///////////////////////////////////////////////////////////////////////////////////////
    ///                                         SERVER
    /// ///////////////////////////////////////////////////////////////////////////////////////
    public class PD_ServerChat : IPacketData
    {
        public enum Type
        {
            GLOBAL,
            FRIENDS,
            FRIEND,
            LOCAL,
            GROUP,
            CLAN
        }
        public string senderName;
        public Type type;
        public string message;
    }

    public class PD_ServerListPlayers : IPacketData {}


    /// ///////////////////////////////////////////////////////////////////////////////////////
    ///                                         GROUP 
    /// ///////////////////////////////////////////////////////////////////////////////////////

    /*
     * init battle
     * start battle
     * drawpile info
     * discardpile info
     * hand info
     * battlestate info
     * cardplayed
     * */

    public class PD_BattleInit : IPacketData
    {
        public List<string> playerList;
        public List<string> monsterList;
        public List<string> monsterList;
    }

    public class PD_BattleStart : IPacketData
    {
        public int randomSeed;
    }

    public class PD_BattlePlayCard : IPacketData
    {
        public string ownerName;
        public string targetName;
        public int cardIndex;
    }

    public class PD_BattlePlayerTurnEnd : IPacketData
    {
        public string name;
    }

    /// ///////////////////////////////////////////////////////////////////////////////////////
    ///                                         PARTY
    /// ///////////////////////////////////////////////////////////////////////////////////////
    public class PD_PartyInvite : IPacketData
    {
        public string inviter;
        public string invity;
        public List<string> partyAccounts;
    }

    public class PD_PartyMemberNew : IPacketData
    {
        public string memberName;
    }

    public class PD_PartyMemberLeave : IPacketData
    {
        public string memberName;
    }

    /// ///////////////////////////////////////////////////////////////////////////////////////
    ///                                         WorldPlayerInfo
    /// ///////////////////////////////////////////////////////////////////////////////////////
    public class PD_WorldPlayerPosition : IPacketData
    {
        public int accountName;
        public Vector2 position;
        public Vector2 velocity;
    }
    public class PD_WorldPlayerInfo : IPacketData
    {
        public int accountName;
        public string info;
    }



    public class PD_SessionShutdown : IPacketData { }


    public class PD_InfoOnlineList : IPacketData
    {
        public List<string> names;
    }
}
