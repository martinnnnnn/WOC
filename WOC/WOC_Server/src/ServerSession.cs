using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WOC_Core;

namespace WOC_Server
{
    public class ServerSession : Session
    {
        TCPServer server;
        public Room room;

        public ServerSession(TCPServer s)
        {
            server = s;
        }

        public override void HandleAPICall(IPacketData data)
        {
            switch (data)
            {
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
                case PD_ServerChat serverChat:
                    HandleAPICall(serverChat);
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
                case PD_ServerDeleteRoom serverListRoom:
                    HandleAPICall(serverListRoom);
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
            }
        }


        public void HandleAPICall(PD_AccountMake data)
        {
            Debug.Assert(account == null);
            string errorMessage = "";

            if (String.IsNullOrEmpty(data.email) || String.IsNullOrEmpty(data.password) || String.IsNullOrEmpty(data.name))
            {
                errorMessage = "Not enough info to create acount.";
            }
            else
            {
                account = new Account()
                {
                    email = data.email,
                    password = data.password,
                    name = data.name,
                    connected = true,
                    session = this
                };

                bool result = server.users.TryAdd(data.email, account);

                if (!result)
                {
                    account = null;
                    errorMessage = "This email is already used.";
                }
                else
                {
                    server.Broadcast(new PD_AccountConnected { name = account.name }, this, true);
                }
            }

            Send(new PD_Validation(data.id, errorMessage), true);
        }

        public void HandleAPICall(PD_AccountModify data)
        {
            if (!AssureConnected(data.id)) return;

            Debug.Assert(account.email == data.oldEmail, "Cannot try to modify a user that's not the one currently connected.");
            string errorMessage = "";

            bool result = server.users.TryRemove(data.oldEmail, out Account user);

            if (result)
            {
                if (!String.IsNullOrEmpty(data.newEmail))
                {
                    account.email = data.newEmail;
                    user.email = data.newEmail;
                }
                if (!String.IsNullOrEmpty(data.newPassword))
                {
                    account.password = data.newPassword;
                    user.password = data.newPassword;
                }
                if (!String.IsNullOrEmpty(data.newName))
                {
                    account.name = data.newName;
                    user.name = data.newName;
                }

                result = server.users.TryAdd(user.email, user);
                errorMessage = result ? "" : "New email already exists.";

                if (result && data.oldName != data.newName)
                {
                    server.Broadcast(new PD_AccountNameModify{ oldName = data.oldName, newName = data.newName}, this, true);
                }
            }
            else
            {
                errorMessage = "Could not find the email.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountDelete data)
        {
            if (!AssureConnected(data.id)) return;

            string errorMessage = "";
            bool result = server.users.TryRemove(data.email, out Account deletingAccount);
            if (result)
            {
                if (deletingAccount.password == data.password)
                {
                    server.Broadcast(new PD_AccountDeleted { name = account.name }, this, true);
                    account = null;
                }
                else
                {
                    errorMessage = "Wrong password.";
                }
            }
            else
            {
                errorMessage = "Could not find matching email.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountConnect data)
        {
            string errorMessage = "";
            bool result = server.users.TryGetValue(data.email, out Account connectingAccount);

            if (result)
            {
                if (connectingAccount.password == data.password)
                {
                    connectingAccount.connected = true;
                    connectingAccount.session = this;
                    account = connectingAccount;
                    server.Broadcast(new PD_AccountConnected { name = account.name }, this, true);
                }
                else
                {
                    errorMessage = "Wrong password.";
                }
            }
            else
            {
                errorMessage = "Could not find matching email.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountDisconnect data)
        {
            if (!AssureConnected(data.id)) return;

            string errorMessage = "";
            bool result = server.users.TryGetValue(data.email, out Account disconnectingAccount);
            if (result)
            {
                server.Broadcast(new PD_AccountDisconnected { name = account.name }, this, true);
                disconnectingAccount.connected = false;
                disconnectingAccount.session = null;
                account = null;
            }
            else
            {
                errorMessage = "Could not find matching email.";
            }

            Send(new PD_Validation(data.id, errorMessage), true);
        }

        public void HandleAPICall(PD_AccountAddFriend data)
        {
            if (!AssureConnected(data.id)) return;

            string errorMessage = "";
            if (account.name != data.name)
            {
                Account newFriend = server.users.FirstOrDefault(acc => acc.Value.name == data.name).Value;

                if (newFriend == null)
                {
                    errorMessage = "Could not find your friend.";
                }
                else if (account.friends.FirstOrDefault(acc => acc == data.name) != default(string))
                {
                    errorMessage = "Already your friend !";
                }
                else
                {
                    account.friends.Add(newFriend.name);
                    newFriend.friends.Add(account.name);
                    newFriend.session?.Send(new PD_AccountAddFriend{ name = account.name });
                }
            }
            else
            {
                errorMessage = "You cannot add yourself.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountRemoveFriend data)
        {
            if (!AssureConnected(data.id)) return;

            string errorMessage = "";
            if (account.name != data.name)
            {
                Account oldFriend = server.users.FirstOrDefault(acc => acc.Value.name == data.name).Value;

                if (oldFriend == null)
                {
                    errorMessage = "Could not find your friend.";
                }
                else if (account.friends.FirstOrDefault(acc => acc == data.name) == default(string))
                {
                    errorMessage = "Not your friend !";
                }
                else
                {
                    account.friends.Remove(oldFriend.name);
                    oldFriend.friends.Remove(account.name);
                    oldFriend.session?.Send(new PD_AccountRemoveFriend { name = account.name });
                }
            }
            else
            {
                errorMessage = "You cannot add yourself.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        //TODO send validation for chat
        public void HandleAPICall(PD_ServerChat data)
        {
            try
            {
                switch (data.type)
                {
                    case PD_ServerChat.Type.LOCAL:
                        if (room == null)
                        {
                            server.Broadcast(data, this);
                        }
                        else
                        {
                            room.Broadcast(data, this);
                        }
                        break;
                    case PD_ServerChat.Type.FRIENDS:
                        //data.message = data.message.Remove(0, 4);
                        server.Broadcast(data, server.sessions.Where(s => account.friends.Contains(s.account.name) && s.account.connected));
                        break;
                    case PD_ServerChat.Type.GLOBAL:
                        server.Broadcast(data, this, true);
                        break;
                    default:
                        LOG.Print("Message type not supported");
                        break;
                }
            }
            catch (Exception)
            {
                LOG.Print("[SERVER] Failed to broadcast message.");
            }
        }

        public void HandleAPICall(PD_AccountAddCharacter data)
        {
            if (!AssureConnected(data.id)) return;

            string errorMessage = "";
            if (account.characters.FirstOrDefault(c => c.Name == data.name) == null)
            {
                account.characters.Add(new Character(data.race, data.category, data.life, data.name));
            }
            else
            {
                errorMessage = "Character name already exists in your roster.";
            }
            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountDeleteCharacter data)
        {
            if (!AssureConnected(data.id)) return;

            string errorMessage = "";
            var toRemove = account.characters.FirstOrDefault(c => c.Name == data.name);
            if (toRemove != null)
            {
                account.characters.Remove(toRemove);
            }
            else
            {
                errorMessage = "Character name could not be found in your roster.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountSetDefaultCharacter data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            var toDefault = account.characters.FirstOrDefault(c => c.Name == data.name);
            if (toDefault != null)
            {
                account.defaultCharacter = toDefault;
            }
            else
            {
                errorMessage = "Character name could not be found in your roster.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountNewDeck data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            if (account.decks.Find(d => d.name == data.name) == null)
            {
                Deck newDeck = new Deck() { name = data.name };
                account.decks.Add(newDeck);
                account.defaultDeck = account.defaultDeck ?? newDeck;
            }
            else
            {
                errorMessage = "A deck with the same name already exists.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountAddCard data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            Deck deck = account.decks.Find(d => d.name == data.deckName);
            if (deck != null)
            {
                if (server.cards.Find(c => c.name == data.cardName) != null)
                {
                    deck.cardNames.Add(data.cardName);
                }
                else
                {
                    errorMessage = "Wrong card name.";
                }
            }
            else
            {
                errorMessage = "Could not find the deck.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }
        

        public void HandleAPICall(PD_AccountRenameDeck data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            Deck deck = account.decks.Find(d => d.name == data.oldName);
            if (deck != null)
            {
                deck.name = data.newName;
            }
            else
            {
                errorMessage = "No deck with this name was found.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }
        public void HandleAPICall(PD_AccountDeleteDeck data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            if (account.decks.RemoveAll(d => d.name == data.name) != 1)
            {
                errorMessage = "Wrong deck name.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }
        public void HandleAPICall(PD_AccountSetDefaultDeck data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            Deck deck = account.decks.Find(d => d.name == data.name);
            if (deck != null)
            {
                account.defaultDeck = deck;
            }
            else
            {
                errorMessage = "No deck with this name was found.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_ServerMakeRoom data)
        {
            string errorMessage = "";

            if (!server.Exists(data.roomName))
            {
                room = server.CreateRoom(data.roomName);
                server.MoveToRoom(room, this);
                data.randomSeed = room.battle.RandomSeed;
                server.Broadcast(data, this, true);
            }
            else
            {
                errorMessage = "Room name already exists";
            }
            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_ServerRenameRoom data)
        {
            string errorMessage = "";
            room = server.rooms.FirstOrDefault(r => data.oldName == r.Name);
            if (room != null)
            {
                room.Name = data.newName;
                server.Broadcast(data, this, true);
            }
            else
            {
                errorMessage = "Could not find room.";
            }
            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_ServerJoinRoom data)
        {
            Debug.Assert(data.userName == account.name);
            string errorMessage = "";

            Room room = server.rooms.FirstOrDefault(r => data.roomName == r.Name);
            if (room != null)
            {
                if (server.MoveToRoom(room, this))
                {
                    server.Broadcast(data, this, true);
                }
                else
                {
                    errorMessage = "Unable to join room.";
                }
            }
            else
            {
                errorMessage = "Could not find room.";
            }
            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_ServerDeleteRoom data)
        {
            string errorMessage = "";

            Room room = server.rooms.FirstOrDefault(r => data.name == r.Name);
            if (room != null)
            {
                room.ForEach(s => server.sessions.Add(s));
                room.ForEach(s => s.room = null);
                room.Clear();
                server.rooms.Remove(room);

                server.Broadcast(data, this, true);
            }
            else
            {
                errorMessage = "Could not find room.";
            }
            Send(new PD_Validation(data.id, errorMessage));
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

        public bool AssureConnected(Guid id)
        {
            if (account == null || !account.connected)
            {
                Send(new PD_Validation(id, "You need to be connected to do this."));
                return false;
            }
            return true;
        }
    }
}
