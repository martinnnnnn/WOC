using System;
using System.Collections.Generic;
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
        public override void HandleIncomingMessage(IPacketData data)
        {
            base.HandleIncomingMessage(data);

            switch (data)
            {
                case PD_NameModify nameModify:
                    Name = nameModify.newName;
                    if (room == null)
                    {
                        server.Broadcast(nameModify, this).Wait();
                    }
                    else
                    {
                        room.Broadcast(nameModify, this).Wait();
                    }
                    break;
                case PD_Chat chat:
                    try
                    {
                        if (chat.message.StartsWith("/all "))
                        {
                            chat.message = chat.message.Remove(0, 5);
                            server.Broadcast(chat, this, true).Wait();
                        }
                        else if (room == null)
                        {
                            server.Broadcast(chat, this).Wait();
                        }
                        else
                        {
                            room.Broadcast(chat, this).Wait();
                        }
                    }
                    catch (Exception)
                    {
                        LOG.Print("[SERVER] Failed to broadcast message.");
                    }
                    break;

                case PD_PlayerAdd player:
                    HandlePlayerAdd(player);
                    break;

                case PD_RoomCreate roomCreate:
                    HandleRoomCreate(roomCreate);
                    break;

                case PD_RoomJoin roomEnter:
                    HandleRoomJoin(roomEnter);
                    break;

                case PD_RoomLeave roomLeave:
                    if (room == null)
                    {
                        room.Remove(this);
                        room.Broadcast(roomLeave, this).Wait();
                        room = null;
                    }
                    server.sessions.Add(this);
                    break;

                case PD_PlayerList playerList:
                    if (string.IsNullOrEmpty(playerList.roomName))
                    {
                        SendAsync(new PD_PlayerList { players = server.sessions.Select(s => s.Name).ToList() }).Wait();
                    }
                    else
                    {
                        var room = server.battleRooms.Find(r => r.Name == playerList.roomName);
                        if (room != null)
                        {
                            SendAsync(new PD_PlayerList { roomName = playerList.roomName, players = room.PlayerList }).Wait();
                        }
                        else
                        {
                            SendAsync(new PD_Validation(playerList.id, "Battle name does not exist.")).Wait();
                        }
                    }
                    break;

                case PD_BattleStart battleStart:
                    if (room.Start())
                    {
                        room.Broadcast(battleStart, this).Wait();
                    }
                    break;

                case PD_RoomList battleList:
                    battleList.rooms = new List<string>();
                    battleList.rooms.AddRange(server.battleRooms.Select(r => r.Name));
                    SendAsync(battleList).Wait();
                    break;

                case PD_CardPlayed cardPlayed:
                    Card card = actor.hand.Get(cardPlayed.cardIndex);
                    Character character = room.battle.Actors.Find(a => a.character.Name == cardPlayed.targetName).character;

                    room.Broadcast(cardPlayed, this).Wait();
                    actor.PlayCard(card, character);
                    break;

                case PD_TurnEnd turnEnd:
                    if (room.battle.GetCurrentActor() == actor)
                    {
                        actor.EndTurn();
                        room.battle.NextActor().StartTurn();
                        room.Broadcast(turnEnd, this).Wait();
                    }
                    break;
            }
        }

        public void HandlePlayerAdd(PD_PlayerAdd player)
        {
            actor = new PlayerActor(
                       new Character(player.charaRace, player.charaCategory, player.charaLife, player.charaName),
                       player.handStartCount,
                       player.name,
                       player.cardsName,
                       player.aggroIncrement,
                       player.manaMax);
            LOG.Print("[SERVER] Player created ? {0}", (actor != null) ? "true" : "false");

            if (room != null)
            {
                if (room.battle.Add(actor))
                {
                    room.Broadcast(player, this).Wait();
                }
            }
        }

        public void HandleRoomJoin(PD_RoomJoin roomJoin)
        {
            if (server.MoveToBattleRoom(roomJoin.roomName, this))
            {
                room = server.battleRooms.Find(r => r.Name == roomJoin.roomName);
                //if (roomJoin.playerInfo != null)
                //{
                //    HandlePlayerAdd(roomJoin.playerInfo as PD_PlayerAdd);
                //}

                roomJoin.randomSeed = room.battle.RandomSeed;
                room.Broadcast(roomJoin).Wait();
            }
            else
            {
                SendAsync(new PD_Validation(roomJoin.id, "Battle name does not exist.")).Wait();
            }
        }

        public void HandleRoomCreate(PD_RoomCreate battleCreate)
        {
            if (server.CreateBattleRoom(battleCreate.name))
            {
                if (server.MoveToBattleRoom(battleCreate.name, this))
                {
                    room = server.battleRooms.Find(r => r.Name == battleCreate.name);
                    room.Broadcast(new PD_RoomJoin
                    {
                        playerName = Name,
                        roomName = battleCreate.name,
                        randomSeed = room.battle.RandomSeed
                    }).Wait();
                }
            }
            else
            {
                SendAsync(new PD_Validation(battleCreate.id, "Room name already exists.")).Wait();
            }
        }
    }
}
