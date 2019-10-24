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
        public PlayerActor actor;

        public ServerSession(TCPServer s)
        {
            server = s;
        }

        public void HandleAPICall(IPacketData data)
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
                case PD_ServerListRoom serverListRoom:
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

            if (String.IsNullOrEmpty(data.email) || String.IsNullOrEmpty(data.password) || String.IsNullOrEmpty(data.name))
            {
                SendAsync(new PD_Validation(data.id,"Not enough info to create acount.")).Wait();
                return;
            }

            account = new Account()
            {
                email = data.email,
                password = data.password,
                name = data.name,
                connected = true,
                session = this
            };

            bool result = server.users.TryAdd(data.email, account);

            if (!result) account = null;

            SendAsync(new PD_Validation(data.id, result ? "" : "This email is already used."), true).Wait();
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
                    server.Broadcast(new PD_AccountNameModify{ oldName = data.oldName, newName = data.newName}, this, true).Wait();
                }
            }
            else
            {
                errorMessage = "Could not find the email.";
            }

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
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
                    server.Broadcast(new PD_AccountDeleted { name = account.name }, this, true).Wait();
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

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
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
                    server.Broadcast(new PD_AccountConnected { name = account.name }, this, true).Wait();
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

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
        }

        public void HandleAPICall(PD_AccountDisconnect data)
        {
            if (!AssureConnected(data.id)) return;

            string errorMessage = "";
            bool result = server.users.TryGetValue(data.email, out Account disconnectingAccount);
            if (result)
            {
                server.Broadcast(new PD_AccountDisconnected { name = account.name }, this, true).Wait();
                disconnectingAccount.connected = false;
                disconnectingAccount.session = null;
                account = null;
            }
            else
            {
                errorMessage = "Could not find matching email.";
            }

            SendAsync(new PD_Validation(data.id, errorMessage), true).Wait();
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
                    newFriend.session?.SendAsync(new PD_AccountAddFriend{ name = account.name });
                }
            }
            else
            {
                errorMessage = "You cannot add yourself.";
            }

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
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
                    oldFriend.session?.SendAsync(new PD_AccountRemoveFriend { name = account.name });
                }
            }
            else
            {
                errorMessage = "You cannot add yourself.";
            }

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
        }

        //TODO send validation for chat
        //TODO chat with friends / room / all
        public void HandleAPICall(PD_ServerChat data)
        {
            try
            {
                if (data.message.StartsWith("/f "))
                {
                    LOG.Print("Talk with friend not implemented");
                }
                if (data.message.StartsWith("/all "))
                {
                    data.message = data.message.Remove(0, 5);
                    server.Broadcast(data, this, true).Wait();
                }
                else if (room == null)
                {
                    server.Broadcast(data, this).Wait();
                }
                else
                {
                    room.Broadcast(data, this).Wait();
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
            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
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

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
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

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
        }

        public void HandleAPICall(PD_AccountNewDeck data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            if (account.decks.Find(d => d.name == data.name) == null)
            {
                Deck newDeck = new Deck() { name = data.name, cardNames = data.cardNames };
                account.decks.Add(newDeck);
                account.defaultDeck = account.defaultDeck ?? newDeck;
            }
            else
            {
                errorMessage = "A deck with the same name already exists.";
            }

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
        }

        public void HandleAPICall(PD_AccountAddCard data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            Deck deck = account.decks.Find(d => d.name == data.deckName);
            if (deck != null)
            {
                deck.cardNames.Add(data.cardName);
            }
            else
            {
                errorMessage = "Could not find the deck.";
            }

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
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

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
        }
        public void HandleAPICall(PD_AccountDeleteDeck data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            if (account.decks.RemoveAll(d => d.name == data.name) != 1)
            {
                errorMessage = "Wrong deck name.";
            }

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
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

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
        }

        public void HandleAPICall(PD_ServerMakeRoom data)
        {
            string errorMessage = "";

            if (!server.Exists(data.name))
            {
                
            }
            else
            {
                errorMessage = "Room name already exists";
            }

            server.CreateBattleRoom(data.name);

            if (server.MoveToBattleRoom(data.name, this))
            {
                room = server.battleRooms.Find(r => r.Name == data.name);

                //account.actor = new PlayerActor(
                //           new Character(player.charaRace, player.charaCategory, player.charaLife, player.charaName),
                //           player.handStartCount,
                //           player.name,
                //           player.aggroIncrement,
                //           player.manaMax);
                LOG.Print("[SERVER] Player created ? {0}", (actor != null) ? "true" : "false");

                //    if (room != null)
                //    {
                //        if (room.battle.Add(actor))
                //        {
                //            actor.AddCards(player.cardsName);
                //            room.Broadcast(player, this).Wait();
                //        }
                //    }

                data.randomSeed = room.battle.RandomSeed;
                room.Broadcast(data).Wait();
            }
            else
            {
                errorMessage = "Battle name does not exist.";
                
            }

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
        }

        public void HandleAPICall(PD_ServerRenameRoom data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_ServerJoinRoom data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_ServerListRoom data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
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
                SendAsync(new PD_Validation(id, "You need to be connected to do this.")).Wait();
                return false;
            }
            return true;
        }

        public override void HandleIncomingMessage(IPacketData data)
        {
            base.HandleIncomingMessage(data);

            HandleAPICall(data);

            //switch (data)
            //{
            //    case PD_NameModify nameModify:
            //        Name = nameModify.newName;
            //        if (room == null)
            //        {
            //            server.Broadcast(nameModify, this).Wait();
            //        }
            //        else
            //        {
            //            room.Broadcast(nameModify, this).Wait();
            //        }
            //        break;
            //    case PD_Chat chat:
            //        try
            //        {
            //            if (chat.message.StartsWith("/all "))
            //            {
            //                chat.message = chat.message.Remove(0, 5);
            //                server.Broadcast(chat, this, true).Wait();
            //            }
            //            else if (room == null)
            //            {
            //                server.Broadcast(chat, this).Wait();
            //            }
            //            else
            //            {
            //                room.Broadcast(chat, this).Wait();
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            LOG.Print("[SERVER] Failed to broadcast message.");
            //        }
            //        break;

            //    case PD_PlayerAdd player:
            //        HandlePlayerAdd(player);
            //        break;

            //    case PD_PNJAdd pnjAdd:
            //        HandlePNJAdd(pnjAdd);
            //        break;

            //    case PD_RoomJoin roomEnter:
            //        HandleRoomJoin(roomEnter);
            //        break;

            //    case PD_RoomLeave roomLeave:
            //        if (room == null)
            //        {
            //            room.Remove(this);
            //            room.Broadcast(roomLeave, this).Wait();
            //            room = null;
            //        }
            //        server.sessions.Add(this);
            //        break;

            //    case PD_PlayerList playerList:
            //        if (string.IsNullOrEmpty(playerList.roomName))
            //        {
            //            SendAsync(new PD_PlayerList { players = server.sessions.Select(s => s.Name).ToList() }).Wait();
            //        }
            //        else
            //        {
            //            var room = server.battleRooms.Find(r => r.Name == playerList.roomName);
            //            if (room != null)
            //            {
            //                SendAsync(new PD_PlayerList { roomName = playerList.roomName, players = room.PlayerList }).Wait();
            //            }
            //            else
            //            {
            //                SendAsync(new PD_Validation(playerList.id, "Battle name does not exist.")).Wait();
            //            }
            //        }
            //        break;

            //    case PD_BattleStart battleStart:
            //        if (room.Start())
            //        {
            //            room.Broadcast(battleStart, this).Wait();
            //        }
            //        break;

            //    case PD_RoomList battleList:
            //        battleList.rooms = new List<string>();
            //        battleList.rooms.AddRange(server.battleRooms.Select(r => r.Name));
            //        SendAsync(battleList).Wait();
            //        break;

            //    case PD_CardPlayed cardPlayed:
            //        Card card = actor.hand.Get(cardPlayed.cardIndex);
            //        Character character = room.battle.Actors.Find(a => a.character.Name == cardPlayed.targetName).character;

            //        room.Broadcast(cardPlayed, this).Wait();
            //        actor.PlayCard(card, character);
            //        break;

            //    case PD_TurnEnd turnEnd:
            //        if (room.battle.GetCurrentActor() == actor)
            //        {
            //            actor.EndTurn();
            //            room.battle.NextActor().StartTurn();
            //            room.Broadcast(turnEnd, this).Wait();
            //        }
            //        break;
            //}
        }

        //public void HandlePNJAdd(PD_PNJAdd pnjAdd)
        //{
        //    var pnj = new PNJActor(new Character(pnjAdd.race, pnjAdd.category, pnjAdd.life), pnjAdd.name, pnjAdd.initiative);

        //    if (room != null)
        //    {
        //        if (room.battle.Add(pnj))
        //        {
        //            room.Broadcast(pnjAdd, this).Wait();
        //        }
        //    }
        //}

        //public void HandlePlayerAdd(PD_PlayerAdd player)
        //{
        //    actor = new PlayerActor(
        //               new Character(player.charaRace, player.charaCategory, player.charaLife, player.charaName),
        //               player.handStartCount,
        //               player.name,
        //               player.aggroIncrement,
        //               player.manaMax);
        //    LOG.Print("[SERVER] Player created ? {0}", (actor != null) ? "true" : "false");

        //    if (room != null)
        //    {
        //        if (room.battle.Add(actor))
        //        {
        //            actor.AddCards(player.cardsName);
        //            room.Broadcast(player, this).Wait();
        //        }
        //    }
        //}

        //public void HandleRoomJoin(PD_RoomJoin roomJoin)
        //{
        //    if (!server.Exists(roomJoin.roomName) && roomJoin.create)
        //    {
        //        server.CreateBattleRoom(roomJoin.roomName);
        //    }
        //    if (server.MoveToBattleRoom(roomJoin.roomName, this))
        //    {
        //        room = server.battleRooms.Find(r => r.Name == roomJoin.roomName);
        //        if (roomJoin.playerInfo != null)
        //        {
        //            HandlePlayerAdd(roomJoin.playerInfo as PD_PlayerAdd);
        //        }

        //        roomJoin.randomSeed = room.battle.RandomSeed;
        //        room.Broadcast(roomJoin).Wait();
        //    }
        //    else
        //    {
        //        SendAsync(new PD_Validation(roomJoin.id, "Battle name does not exist.")).Wait();
        //    }
        //}
    }
}
