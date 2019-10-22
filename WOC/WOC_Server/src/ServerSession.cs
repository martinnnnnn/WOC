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

        public BattleRoom room;
        public PlayerActor actor;

        public ServerSession(TCPServer s)
        {
            server = s;
        }

        public void HandleAPICall(IPacketData data)
        {
            switch (data)
            {
                case PD_AccountMake userMake:
                    HandleAPICall(userMake);
                    break;
                case PD_AccountModify userModify:
                    HandleAPICall(userModify);
                    break;
                case PD_AccountDelete userDelete:
                    HandleAPICall(userDelete);
                    break;
                case PD_AccountConnect userConnect:
                    HandleAPICall(userConnect);
                    break;
                case PD_AccountDisconnect userDisconnect:
                    HandleAPICall(userDisconnect);
                    break;
                case PD_AccountAddFriend userAddFriend:
                    HandleAPICall(userAddFriend);
                    break;
                case PD_AccountRemoveFriend userRemoveFriend:
                    HandleAPICall(userRemoveFriend);
                    break;
                case PD_AccountAddCharacter userAddCharacter:
                    HandleAPICall(userAddCharacter);
                    break;
                case PD_AccountDeleteCharacter userDeleteCharacter:
                    HandleAPICall(userDeleteCharacter);
                    break;
                case PD_AccountSetDefaultCharacter userSetDefaultCharacter:
                    HandleAPICall(userSetDefaultCharacter);
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
                case PD_RoomBattleInit roomBattleInit:
                    HandleAPICall(roomBattleInit);
                    break;
                case PD_RoomBattleStart roomBattleStart:
                    HandleAPICall(roomBattleStart);
                    break;
                case PD_BattleCardPlayed battleCardPlayed:
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
            if (String.IsNullOrEmpty(data.email) || String.IsNullOrEmpty(data.password) || String.IsNullOrEmpty(data.name))
            {
                SendAsync(new PD_Validation(data.id,"Not enough info to create acount.")).Wait();
                return;
            }

            bool result = server.users.TryAdd(data.email, new Account()
            {
                email = data.email,
                password = data.password,
                name = data.name
            });

            SendAsync(new PD_Validation(data.id, result ? "" : "This email is already used.")).Wait();
        }

        public void HandleAPICall(PD_AccountModify data)
        {
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
            string errorMessage = "";
            bool result = server.users.TryGetValue(data.email, out Account disconnectingAccount);
            if (result)
            {
                server.Broadcast(new PD_AccountDisconnected { name = account.name }, this, true).Wait();
                disconnectingAccount.connected = false;
                account = null;
            }
            else
            {
                errorMessage = "Could not find matching email.";
            }

            SendAsync(new PD_Validation(data.id, errorMessage)).Wait();
        }

        public void HandleAPICall(PD_AccountAddFriend data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_AccountRemoveFriend data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_AccountAddCharacter data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_AccountDeleteCharacter data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_AccountSetDefaultCharacter data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
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
        public void HandleAPICall(PD_ServerMakeRoom data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
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
        public void HandleAPICall(PD_RoomBattleInit data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_RoomBattleStart data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_BattleCardPlayed data)
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
