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
        public string Name = "default";

        TCPServer server;

        BattleRoom room;
        public PlayerActor actor;

        public ServerSession(TCPServer s)
        {
            server = s;
        }
        public override void HandleIncomingMessage(IPacketData data)
        {
            LOG.Print("[SERVER] received a packet. {0}", data);
            base.HandleIncomingMessage(data);

            switch (data)
            {
                case PD_NameModify nameModify:
                    Name = nameModify.name;
                    break;
                case PD_Chat chat:
                    try
                    {
                        server.Broadcast(chat).Wait();
                    }
                    catch (Exception)
                    {
                        LOG.Print("[SERVER] Failed to broadcast message.");
                    }
                    break;

                case PD_PlayerAdd player:
                    actor = new PlayerActor(
                        room.battle, 
                        new Character(player.charaRace, player.charaCategory, player.charaLife, player.charaName),
                        new Hand(player.handStartCount, player.handMaxCount),
                        player.name,
                        player.cardsName, 
                        player.aggroIncrement, 
                        player.manaMax);
                    LOG.Print("[SERVER] Player created ? {0}", (actor != null) ? "true" : "false");

                    if (room.battle.Add(actor))
                    {
                        server.Broadcast(player, this).Wait();
                    }
                    break;

                case PD_BattleCreate battleCreate:
                    if (server.CreateBattleRoom(battleCreate.name))
                    {
                        if (server.MoveToBattleRoom(battleCreate.name, this))
                        {
                            room = server.battleRooms.Find(r => r.Name == battleCreate.name);
                        }
                    }
                    else
                    {
                        SendAsync(new PD_Validate(battleCreate.id, "Battle name already exists")).Wait();
                    }
                    break;

                case PD_BattleJoin battleEnter:
                    if (!server.MoveToBattleRoom(battleEnter.name, this))
                    {
                        SendAsync(new PD_Validate(battleEnter.id, "[SERVER] Battle name does not exist")).Wait();
                    }
                    break;

                case PD_BattleLeave battleLeave:
                    room.Remove(this);
                    server.sessions.Add(this);
                    break;

                case PD_PlayerList playerList:
                    switch (playerList.location)
                    {
                        case PD_PlayerList.Location.ROOM:
                            if (room != null) SendAsync(new PD_PlayerList { players = room.PlayerList }).Wait();
                            break;
                        case PD_PlayerList.Location.SERVER:
                            SendAsync(new PD_PlayerList { players = server.sessions.Select(s => s.Name).ToList() }).Wait();
                            break;
                        case PD_PlayerList.Location.BOTH:
                            break;
                    }
                    break;

                case PD_BattleStart battleStart:
                    if (room.Start())
                    {
                        server.Broadcast(battleStart, this).Wait();
                    }
                    break;

                case PD_BattleList battleList:
                    battleList.rooms = new List<string>();
                    battleList.rooms.AddRange(server.battleRooms.Select(r => r.Name));
                    SendAsync(battleList).Wait();
                    break;

                case PD_CardPlayed cardPlayed:
                    Card card = actor.hand.Get(cardPlayed.cardIndex);
                    Character character = room.battle.Actors.Find(a => a.character.Name == cardPlayed.targetName).character;

                    LOG.Print("[SERVER] Found right card ? {0}", (card.name == cardPlayed.cardName) ? "true" : "false");
                    LOG.Print("[SERVER] Found right character ? {0}", (character != null) ? "true" : "false");

                    if (actor.PlayCard(card, character))
                    {
                        server.Broadcast(cardPlayed, this).Wait();
                    }
                    break;

                case PD_TurnEnd turnEnd:
                    if (room.battle.GetCurrentActor() == actor)
                    {
                        actor.EndTurn();
                        room.battle.NextActor().StartTurn();
                        server.Broadcast(turnEnd, this).Wait();
                    }
                    break;
            }
        }
    }
}
