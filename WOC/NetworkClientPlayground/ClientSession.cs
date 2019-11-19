//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WOC_Core;

//namespace Playground
//{
//    public class ClientSession : Session
//    {
//        public List<string> onlineAccountNames = new List<string>();

//        public ConcurrentDictionary<Guid, IPacketData> awaitingValidations = new ConcurrentDictionary<Guid, IPacketData>();

//        public override void HandleAPICall(IPacketData data)
//        {
//            switch (data)
//            {
//                case PD_Validation validation:
//                    HandleAPICall(validation);
//                    break;
//                case PD_AccountNameModify accountNameModify:
//                    HandleAPICall(accountNameModify);
//                    break;
//                case PD_ServerChat serverChat:
//                    HandleAPICall(serverChat);
//                    break;
//                case PD_AccountConnected accountConnected:
//                    HandleAPICall(accountConnected);
//                    break;
//                case PD_AccountDisconnected accountdisconnected:
//                    HandleAPICall(accountdisconnected);
//                    break;
//                case PD_AccountDeleted accountDeleted:
//                    HandleAPICall(accountDeleted);
//                    break;
//                case PD_AccountMake accountMake:
//                    HandleAPICall(accountMake);
//                    break;
//                case PD_AccountModify accountModify:
//                    HandleAPICall(accountModify);
//                    break;
//                case PD_AccountDelete accountDelete:
//                    HandleAPICall(accountDelete);
//                    break;
//                case PD_AccountConnect accountConnect:
//                    HandleAPICall(accountConnect);
//                    break;
//                case PD_AccountDisconnect accountDisconnect:
//                    HandleAPICall(accountDisconnect);
//                    break;
//                case PD_AccountAddFriend accountAddFriend:
//                    HandleAPICall(accountAddFriend);
//                    break;
//                case PD_AccountRemoveFriend accountRemoveFriend:
//                    HandleAPICall(accountRemoveFriend);
//                    break;
//                case PD_AccountNewDeck accountNewDeck:
//                    HandleAPICall(accountNewDeck);
//                    break;
//                case PD_AccountAddCard accountAddCard:
//                    HandleAPICall(accountAddCard);
//                    break;
//                case PD_AccountRenameDeck accountRenameDeck:
//                    HandleAPICall(accountRenameDeck);
//                    break;
//                case PD_AccountDeleteDeck accountDeleteDeck:
//                    HandleAPICall(accountDeleteDeck);
//                    break;
//                case PD_AccountSetCurrentDeck accountSetCurrentDeck:
//                    HandleAPICall(accountSetCurrentDeck);
//                    break;
//                case PD_ServerListPlayers serverListPlayers:
//                    HandleAPICall(serverListPlayers);
//                    break;
//                default:
//                    Debug.Assert(false, "API call not implemented !!");
//                    break;
//            }
//        }

//        public void HandleAPICall(PD_Validation data)
//        {
//            bool result = awaitingValidations.TryRemove(data.validationId, out IPacketData validatedPacket);

//            if (!result)
//            {
//                Console.WriteLine("Could not find packet awaiting validation.");
//                return;
//            }

//            if (!data.isValid)
//            {
//                Console.WriteLine("[CLIENT] Validation denied for packet {0} -> {1}", validatedPacket.GetType().Name, data.errorMessage);
//                return;
//            }

//            Console.WriteLine("[CLIENT] Applying validation for packet {0}, still {1} packet awaiting validation.", validatedPacket.GetType().Name, awaitingValidations.Count);
//            HandleAPICall(validatedPacket);
//        }

//        public void HandleAPICall(PD_AccountNameModify data)
//        {
//            Console.WriteLine("[CLIENT] Player {0} changed his name to {1}", data.oldName, data.newName);
//        }

//        public void HandleAPICall(PD_ServerChat data)
//        {
//            Console.WriteLine(data.senderName + " : " + data.message);
//        }
//        public void HandleAPICall(PD_AccountConnected data)
//        {
//            Console.WriteLine("Player {0} connected.", data.name);
//            onlineAccountNames.Add(data.name);
//        }

//        public void HandleAPICall(PD_AccountDisconnected data)
//        {
//            Console.WriteLine("Player {0} disconnected.", data.name);
//            onlineAccountNames.Remove(data.name);
//        }
//        public void HandleAPICall(PD_AccountDeleted data)
//        {
//            Console.WriteLine("Player {0} deleted.", data.name);
//        }

//        public void HandleAPICall(PD_AccountAddFriend data)
//        {
//            Console.WriteLine("{0} added you as a friend. Adding to your friends list...", data.name);
//            account.friends.Add(data.name);
//        }
//        public void HandleAPICall(PD_AccountRemoveFriend data)
//        {
//            Console.WriteLine("{0} removed you as a friend. Removing from your friends list...", data.name);
//            account.friends.Remove(data.name);
//        }

//        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//        public void HandleAPICall(PD_AccountMake data)
//        {
//            account = new Account()
//            {
//                email = data.email,
//                password = data.password,
//                name = data.name,
//                connected = true
//            };
//            Console.WriteLine("Your account has been created.", data.name);
//        }

//        public void HandleAPICall(PD_AccountModify data)
//        {
//            if (!String.IsNullOrEmpty(data.newEmail))
//            {
//                account.email = data.newEmail;
//            }
//            if (!String.IsNullOrEmpty(data.newPassword))
//            {
//                account.password = data.newEmail;
//            }
//            if (!String.IsNullOrEmpty(data.newName))
//            {
//                account.name = data.newName;
//            }
//            Console.WriteLine("The modifications have been applied.");
//        }

//        public void HandleAPICall(PD_AccountDelete data)
//        {
//            account = null;
//            Console.WriteLine("Your account has been deleted.");
//        }

//        public void HandleAPICall(PD_AccountConnect data)
//        {
//            account.connected = true;
//            Console.WriteLine("Connection successful.");
//        }

//        public void HandleAPICall(PD_AccountDisconnect data)
//        {
//            account.connected = false;
//            Console.WriteLine("[CLIENT] Disconnected");
//        }

//        public void HandleAPICall(PD_AccountNewDeck data)
//        {
//            Deck deck = new Deck() { name = data.name };
//            account.currentDeck = account.currentDeck ?? deck;
//            account.decks.Add(deck);
//            Console.WriteLine("Deck {0} created.", data.name);
//        }
//        public void HandleAPICall(PD_AccountAddCard data)
//        {
//            account.decks.Find(d => d.name == data.deckName).cardNames.Add(data.cardName);
//        }
//        public void HandleAPICall(PD_AccountRenameDeck data)
//        {
//            Deck deck = account.decks.Find(d => d.name == data.oldName);
//            deck.name = data.newName;
//            Console.WriteLine("Deck {0} renamed to {1}.", data.oldName, data.newName);
//        }
//        public void HandleAPICall(PD_AccountDeleteDeck data)
//        {
//            account.decks.RemoveAll(d => d.name == data.name);
//            Console.WriteLine("Deck {0} deleted.", data.name);
//        }
//        public void HandleAPICall(PD_AccountSetCurrentDeck data)
//        {
//            Deck deck = account.decks.Find(d => d.name == data.name);
//            account.currentDeck = deck;
//            Console.WriteLine("Deck {0} i now default.", data.name);
//        }

//        public void HandleAPICall(PD_ServerListPlayers data)
//        {
//            Debug.Assert(false, "NOT IMPLEMENTED YET.");
//        }
//    }
//}
