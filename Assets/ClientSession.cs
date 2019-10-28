using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOC_Core;
using UnityEditor;

namespace WOC_Client
{
    public class ClientSession : Session
    {
        //public PlayerActor currentActor;
        //public List<PlayerActor> actors = new List<PlayerActor>();
        public List<Room> liveRooms = new List<Room>();
        public List<string> onlineAccountNames = new List<string>();

        public ConcurrentDictionary<Guid, IPacketData> awaitingValidations = new ConcurrentDictionary<Guid, IPacketData>();

        public Room room = null;
        //public Battle battle;

        public void HandleAPICall(IPacketData data)
        {
            switch (data)
            {
                case PD_Validation validation:
                    HandleAPICall(validation);
                    break;
                case PD_AccountNameModify accountNameModify:
                    HandleAPICall(accountNameModify);
                    break;
                case PD_ServerChat serverChat:
                    HandleAPICall(serverChat);
                    break;
                case PD_AccountConnected accountConnected:
                    HandleAPICall(accountConnected);
                    break;
                case PD_AccountDisconnected accountdisconnected:
                    HandleAPICall(accountdisconnected);
                    break;
                case PD_AccountDeleted accountDeleted:
                    HandleAPICall(accountDeleted);
                    break;
                case PD_AccountMake accountMake:
                    HandleAPICall(accountMake);
                    break;
                case PD_AccountModify accountModify:
                    HandleAPICall(accountModify);
                    break;
                case PD_AccountDelete accountDelete:
                    HandleAPICall(accountDelete);
                    break;
                case PD_AccountConnect accountConnect:
                    HandleAPICall(accountConnect);
                    break;
                case PD_AccountDisconnect accountDisconnect:
                    HandleAPICall(accountDisconnect);
                    break;
                case PD_AccountAddFriend accountAddFriend:
                    HandleAPICall(accountAddFriend);
                    break;
                case PD_AccountRemoveFriend accountRemoveFriend:
                    HandleAPICall(accountRemoveFriend);
                    break;
                case PD_AccountAddCharacter accountAddCharacter:
                    HandleAPICall(accountAddCharacter);
                    break;
                case PD_AccountModifyCharacter accountModifyCharacter:
                    HandleAPICall(accountModifyCharacter);
                    break;
                case PD_AccountDeleteCharacter accountDeleteCharacter:
                    HandleAPICall(accountDeleteCharacter);
                    break;
                case PD_AccountSetDefaultCharacter accountSetDefaultCharacter:
                    HandleAPICall(accountSetDefaultCharacter);
                    break;
                case PD_AccountNewDeck accountNewDeck:
                    HandleAPICall(accountNewDeck);
                    break;
                case PD_AccountAddCard accountAddCard:
                    HandleAPICall(accountAddCard);
                    break;
                case PD_AccountRenameDeck accountRenameDeck:
                    HandleAPICall(accountRenameDeck);
                    break;
                case PD_AccountDeleteDeck accountDeleteDeck:
                    HandleAPICall(accountDeleteDeck);
                    break;
                case PD_AccountSetDefaultDeck accountSetDefaultDeck:
                    HandleAPICall(accountSetDefaultDeck);
                    break;
                case PD_ServerMakeRoom serverMakeRoom:
                    HandleAPICall(serverMakeRoom);
                    break;
                case PD_ServerRenameRoom serverRenameRoom:
                    HandleAPICall(serverRenameRoom);
                    break;
                case PD_ServerJoinRoom serverJoinRoom:
                    HandleAPICall(serverJoinRoom);
                    break;
                case PD_ServerDeleteRoom serverRemoveRoom:
                    HandleAPICall(serverRemoveRoom);
                    break;
                case PD_ServerListPlayers serverListPlayers:
                    HandleAPICall(serverListPlayers);
                    break;
                case PD_RoomAddPNJ roomAddPNJ:
                    HandleAPICall(roomAddPNJ);
                    break;
                case PD_RoomInitBattle roomBattleInit:
                    HandleAPICall(roomBattleInit);
                    break;
                case PD_RoomStartBattle roomBattleStart:
                    HandleAPICall(roomBattleStart);
                    break;
                case PD_BattlePlayCard battleCardPlayed:
                    HandleAPICall(battleCardPlayed);
                    break;
                case PD_BattleEndTurn battleEndTurn:
                    HandleAPICall(battleEndTurn);
                    break;
                case PD_BattleState battleState:
                    HandleAPICall(battleState);
                    break;
                default:
                    Debug.Assert(false, "API call not implemented !!");
                    break;
            }
        }

        public void HandleAPICall(PD_Validation data)
        {
            bool result = awaitingValidations.TryRemove(data.validationId, out IPacketData validatedPacket);

            if (!result)
            {
                UnityEngine.Debug.Log("Could not find packet awaiting validation.");
                return;
            }

            if (!data.isValid)
            {
                UnityEngine.Debug.Log("[CLIENT] Validation denied for packet " + validatedPacket.GetType().Name + " -> " + data.errorMessage);
                return;
            }

            UnityEngine.Debug.Log("[CLIENT] Applying validation for packet " + validatedPacket.GetType().Name + ", still " + awaitingValidations.Count + " packet awaiting validation.");
            HandleAPICall(validatedPacket);
        }

        public void HandleAPICall(PD_AccountNameModify data)
        {
            UnityEngine.Debug.Log("[CLIENT] Player " + data.oldName + " changed his name to " + data.newName);
        }

        public void HandleAPICall(PD_ServerChat data)
        {
            UnityEngine.Debug.Log(data.senderName + " : " + data.message);
        }
        public void HandleAPICall(PD_AccountConnected data)
        {
            UnityEngine.Debug.Log("Player " + data.name + " connected.");
            onlineAccountNames.Add(data.name);
        }

        public void HandleAPICall(PD_AccountDisconnected data)
        {
            UnityEngine.Debug.Log("Player " + data.name + " disconnected.");
            onlineAccountNames.Remove(data.name);
        }
        public void HandleAPICall(PD_AccountDeleted data)
        {
            UnityEngine.Debug.Log("Player " + data.name + " deleted.");
        }

        public void HandleAPICall(PD_AccountAddFriend data)
        {
            UnityEngine.Debug.Log(data.name + "{0} added you as a friend. Adding to your friends list...");
            account.friends.Add(data.name);
        }
        public void HandleAPICall(PD_AccountRemoveFriend data)
        {
            UnityEngine.Debug.Log(data.name + " removed you as a friend. Removing from your friends list...");
            account.friends.Remove(data.name);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void HandleAPICall(PD_AccountMake data)
        {
            account = new Account()
            {
                email = data.email,
                password = data.password,
                name = data.name,
                connected = true
            };
            UnityEngine.Debug.Log("Your account has been created.");
        }

        public void HandleAPICall(PD_AccountModify data)
        {
            if (!String.IsNullOrEmpty(data.newEmail))
            {
                account.email = data.newEmail;
            }
            if (!String.IsNullOrEmpty(data.newPassword))
            {
                account.password = data.newEmail;
            }
            if (!String.IsNullOrEmpty(data.newName))
            {
                account.name = data.newName;
            }
            UnityEngine.Debug.Log("The modifications have been applied.");
        }

        public void HandleAPICall(PD_AccountDelete data)
        {
            account = null;
            UnityEngine.Debug.Log("Your account has been deleted.");
        }

        public void HandleAPICall(PD_AccountConnect data)
        {
            account.connected = true;
            UnityEngine.Debug.Log("Connection successful.");
        }

        public void HandleAPICall(PD_AccountDisconnect data)
        {
            account.connected = false;
            UnityEngine.Debug.Log("[CLIENT] Disconnected");
        }
        public void HandleAPICall(PD_AccountAddCharacter data)
        {
            account.characters.Add(new Character(data.race, data.category, data.life, data.name));
            UnityEngine.Debug.Log("Character " + data.name + " added successfuly.");
        }
        public void HandleAPICall(PD_AccountModifyCharacter data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }

        public void HandleAPICall(PD_AccountDeleteCharacter data)
        {
            account.characters.RemoveAll(c => c.Name == data.name);
            UnityEngine.Debug.Log("Character " + data.name + " deleted successfuly.");

        }
        public void HandleAPICall(PD_AccountSetDefaultCharacter data)
        {
            account.SetDefaultCharacter(data.name);
        }
        public void HandleAPICall(PD_AccountNewDeck data)
        {
            Deck deck = new Deck() { name = data.name };
            account.defaultDeck = account.defaultDeck ?? deck;
            account.decks.Add(deck);
            UnityEngine.Debug.Log("Deck " + data.name + " created.");
        }
        public void HandleAPICall(PD_AccountAddCard data)
        {
            account.decks.Find(d => d.name == data.deckName).cardNames.Add(data.cardName);
        }
        public void HandleAPICall(PD_AccountRenameDeck data)
        {
            Deck deck = account.decks.Find(d => d.name == data.oldName);
            deck.name = data.newName;
            UnityEngine.Debug.Log("Deck " + data.oldName + " renamed to " + data.newName + ".");
        }
        public void HandleAPICall(PD_AccountDeleteDeck data)
        {
            account.decks.RemoveAll(d => d.name == data.name);
            UnityEngine.Debug.Log("Deck " + data.name + " deleted.");
        }
        public void HandleAPICall(PD_AccountSetDefaultDeck data)
        {
            Deck deck = account.decks.Find(d => d.name == data.name);
            account.defaultDeck = deck;
            UnityEngine.Debug.Log("Deck " + data.name + " is now default.");
        }

        public void HandleAPICall(PD_ServerMakeRoom data)
        {
            Room newRoom = new Room(data.roomName, data.randomSeed);
            liveRooms.Add(newRoom);
            UnityEngine.Debug.Log("Room " + data.roomName + " created.");
            if (data.creatorName == account.name)
            {
                room = newRoom;
                UnityEngine.Debug.Log("Room " + data.roomName + " joined.");
            }
        }
        public void HandleAPICall(PD_ServerRenameRoom data)
        {
            Room renamedRoom = liveRooms.FirstOrDefault(r => r.name == data.oldName);
            if (renamedRoom != null)
            {
                renamedRoom.name = data.newName;
                UnityEngine.Debug.Log("Room " + data.oldName + " renamed to " + data.newName + ".");
            }
        }
        public void HandleAPICall(PD_ServerJoinRoom data)
        {
            if (data.userName == account.name)
            {
                room = liveRooms.Find(r => r.name == data.roomName);
                UnityEngine.Debug.Log("Room " + data.roomName + " joined");
            }
            else if (room != null && room.name == data.roomName)
            {
                UnityEngine.Debug.Log(data.userName + " just joined!");
            }
        }
        public void HandleAPICall(PD_ServerDeleteRoom data)
        {
            if (room != null && room.name == data.name)
            {
                UnityEngine.Debug.Log("Your room just got deleted!");
                room = null;
            }
            else
            {
                UnityEngine.Debug.Log("Room " + data.name + " got deleted.");
            }

            liveRooms.RemoveAll(r => r.name == data.name);
        }
        public void HandleAPICall(PD_ServerListPlayers data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_RoomAddPNJ data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_RoomInitBattle data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_RoomStartBattle data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_BattlePlayCard data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_BattleEndTurn data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_BattleState data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }

        public override void HandleIncomingMessage(IPacketData data)
        {
            base.HandleIncomingMessage(data);

            HandleAPICall(data);
        }
    }
}
