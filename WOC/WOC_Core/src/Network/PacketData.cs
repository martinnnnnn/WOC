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
        //public string name;
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

    public class PD_AccountAddCharacter : IPacketData
    {
        public string name;
        public Character.Race race;
        public Character.Category category;
        public int life;
    }

    public class PD_AccountModifyCharacter : IPacketData
    {
        public string name;
        public Character.Race race;
        public Character.Category category;
        public int life;
    }

    public class PD_AccountDeleteCharacter : IPacketData
    {
        public string name;
    }

    public class PD_AccountSetDefaultCharacter : IPacketData
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

    public class PD_AccountSetDefaultDeck : IPacketData
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
            LOCAL
        }
        public string senderName;
        public Type type;
        public string message;
    }

    public class PD_ServerMakeRoom : IPacketData
    {
        public string roomName;
        public string creatorName;
        public int randomSeed;
    }

    public class PD_ServerRenameRoom : IPacketData
    {
        public string oldName;
        public string newName;
    }

    public class PD_ServerJoinRoom : IPacketData
    {
        public string roomName;
        public string userName;
    }

    public class PD_ServerDeleteRoom : IPacketData
    {
        public string name;
    }

    public class PD_ServerListPlayers : IPacketData
    {
        public string roomName;
    }


    /// ///////////////////////////////////////////////////////////////////////////////////////
    ///                                         ROOM
    /// ///////////////////////////////////////////////////////////////////////////////////////

    public class PD_RoomAddPNJ : IPacketData
    {
        public string name;
        public int life;
        public Character.Race race;
        public Character.Category category;
        public int initiative;
    }

    public class PD_RoomInitBattle : IPacketData { }
    public class PD_RoomStartBattle : IPacketData { }


    /// ///////////////////////////////////////////////////////////////////////////////////////
    ///                                         BATTLE
    /// ///////////////////////////////////////////////////////////////////////////////////////

    public class PD_BattlePlayCard : IPacketData
    {
        public string ownerName;
        public string targetName;
        public int cardIndex;
        public string cardName;
    }

    public class PD_BattleEndTurn : IPacketData { }

    public class PD_BattleState : IPacketData { }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class PD_SessionShutdown : IPacketData { }


    //public class PD_SessionConnect : IPacketData
    //{
    //    public string name;
    //}
    //public class PD_SessionDisconnect : IPacketData
    //{
    //    public string name;
    //}

    //public class PD_NameModify : IPacketData
    //{
    //    public string oldName;
    //    public string newName;
    //}
    //public class PD_Chat : IPacketData
    //{
    //    public string senderName;
    //    public string message;
    //}

    //public class PD_RoomJoin : IPacketData
    //{
    //    public string playerName;
    //    public string roomName;
    //    public int randomSeed;
    //    public bool create;
    //}
    //public class PD_RoomLeave : IPacketData
    //{
    //    public string name;
    //}
    //public class PD_BattleStart : IPacketData { }

    //public class PD_RoomList : IPacketData
    //{
    //    public List<string> rooms;
    //}
    
    //public class PD_PlayerList : IPacketData
    //{
    //    public string roomName;
    //    public List<string> players;
    //}
    //public class PD_TurnEnd : IPacketData { }

    //public class PD_PNJAdd : IPacketData
    //{
    //    public string name;
    //    public int life;
    //    public Character.Race race;
    //    public Character.Category category;
    //    public int initiative;
    //}

    //public class PD_BattlePlayerAdd : IPacketData
    //{
    //    public string playerName;
    //    public string oldCharacterName;
    //    public string newCharacterName;
    //}

    //public class PD_SessionPlayerAdd : IPacketData
    //{
    //    public string               name;
    //    public Character.Race       charaRace;
    //    public Character.Category   charaCategory;
    //    public int                  charaLife;
    //    public string               charaName;
    //    public int                  handStartCount;
    //    public List<string>         cardsName;
    //    public int                  aggroIncrement;
    //    public int                  manaMax;
    //}


    //public class PD_CardPlayed : IPacketData
    //{
    //    public string ownerName;
    //    public string targetName;
    //    public int cardIndex;
    //    public string cardName;
    //}
}
