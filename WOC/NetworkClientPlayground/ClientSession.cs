using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOC_Core;

namespace Playground
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

        public override void HandleAPICall(IPacketData data)
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
                Console.WriteLine("Could not find packet awaiting validation.");
                return;
            }

            if (!data.isValid)
            {
                Console.WriteLine("[CLIENT] Validation denied for packet {0} -> {1}", validatedPacket.GetType().Name, data.errorMessage);
                return;
            }

            Console.WriteLine("[CLIENT] Applying validation for packet {0}, still {1} packet awaiting validation.", validatedPacket.GetType().Name, awaitingValidations.Count);
            HandleAPICall(validatedPacket);
        }

        public void HandleAPICall(PD_AccountNameModify data)
        {
            Console.WriteLine("[CLIENT] Player {0} changed his name to {1}", data.oldName, data.newName);
        }

        public void HandleAPICall(PD_ServerChat data)
        {
            Console.WriteLine(data.senderName + " : " + data.message);
        }
        public void HandleAPICall(PD_AccountConnected data)
        {
            Console.WriteLine("Player {0} connected.", data.name);
            onlineAccountNames.Add(data.name);
        }

        public void HandleAPICall(PD_AccountDisconnected data)
        {
            Console.WriteLine("Player {0} disconnected.", data.name);
            onlineAccountNames.Remove(data.name);
        }
        public void HandleAPICall(PD_AccountDeleted data)
        {
            Console.WriteLine("Player {0} deleted.", data.name);
        }

        public void HandleAPICall(PD_AccountAddFriend data)
        {
            Console.WriteLine("{0} added you as a friend. Adding to your friends list...", data.name);
            account.friends.Add(data.name);
        }
        public void HandleAPICall(PD_AccountRemoveFriend data)
        {
            Console.WriteLine("{0} removed you as a friend. Removing from your friends list...", data.name);
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
            Console.WriteLine("Your account has been created.", data.name);
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
            Console.WriteLine("The modifications have been applied.");
        }

        public void HandleAPICall(PD_AccountDelete data)
        {
            account = null;
            Console.WriteLine("Your account has been deleted.");
        }

        public void HandleAPICall(PD_AccountConnect data)
        {
            account.connected = true;
            Console.WriteLine("Connection successful.");
        }

        public void HandleAPICall(PD_AccountDisconnect data)
        {
            account.connected = false;
            Console.WriteLine("[CLIENT] Disconnected");
        }
        public void HandleAPICall(PD_AccountAddCharacter data)
        {
            account.characters.Add(new Character(data.race, data.category, data.life, data.name));
            Console.WriteLine("Character {0} added successfuly.", data.name);
        }
        public void HandleAPICall(PD_AccountModifyCharacter data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }

        public void HandleAPICall(PD_AccountDeleteCharacter data)
        {
            account.characters.RemoveAll(c => c.Name == data.name);
            Console.WriteLine("Character {0} deleted successfuly.", data.name);

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
            Console.WriteLine("Deck {0} created.", data.name);
        }
        public void HandleAPICall(PD_AccountAddCard data)
        {
            account.decks.Find(d => d.name == data.deckName).cardNames.Add(data.cardName);
        }
        public void HandleAPICall(PD_AccountRenameDeck data)
        {
            Deck deck = account.decks.Find(d => d.name == data.oldName);
            deck.name = data.newName;
            Console.WriteLine("Deck {0} renamed to {1}.", data.oldName, data.newName);
        }
        public void HandleAPICall(PD_AccountDeleteDeck data)
        {
            account.decks.RemoveAll(d => d.name == data.name);
            Console.WriteLine("Deck {0} deleted.", data.name);
        }
        public void HandleAPICall(PD_AccountSetDefaultDeck data)
        {
            Deck deck = account.decks.Find(d => d.name == data.name);
            account.defaultDeck = deck;
            Console.WriteLine("Deck {0} i now default.", data.name);
        }

        public void HandleAPICall(PD_ServerMakeRoom data)
        {
            Room newRoom = new Room(data.roomName, data.randomSeed);
            liveRooms.Add(newRoom);
            Console.WriteLine("Room {0} created.", data.roomName);
            if (data.creatorName == account.name)
            {
                room = newRoom;
                Console.WriteLine("Room {0} joined.", data.roomName);
            }
        }
        public void HandleAPICall(PD_ServerRenameRoom data)
        {
            Room renamedRoom = liveRooms.FirstOrDefault(r => r.name == data.oldName);
            if (renamedRoom != null)
            {
                renamedRoom.name = data.newName;
                Console.WriteLine("Room {0} renamed to {1}.", data.oldName, data.newName);
            }
        }
        public void HandleAPICall(PD_ServerJoinRoom data)
        {
            if (data.userName == account.name)
            {
                room = liveRooms.Find(r => r.name == data.roomName);
                Console.WriteLine("Room {0} joined", data.roomName);
            }
            else if (room != null && room.name == data.roomName)
            {
                Console.WriteLine("{0} just joined!", data.userName);
            }
        }
        public void HandleAPICall(PD_ServerDeleteRoom data)
        {
            if (room != null && room.name == data.name)
            {
                Console.WriteLine("Your room just got deleted!");
                room = null;
            }
            else
            {
                Console.WriteLine("Room {0} got deleted.", data.name);
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
    }
}
