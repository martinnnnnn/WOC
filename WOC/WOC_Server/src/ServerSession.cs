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
        Battle battle;
        public PlayerActor actor;

        public ServerSession(TCPServer s, Battle b)
        {
            server = s;
            battle = b;
        }
        public override void HandleIncomingMessage(IPacketData data)
        {
            LOG.Print("[SERVER] received a packet. {0}", data);
            base.HandleIncomingMessage(data);

            switch (data)
            {
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
                        battle, 
                        new Character(player.charaRace, player.charaCategory, player.charaLife, player.charaName),
                        new Hand(player.handStartCount, player.handMaxCount),
                        player.name,
                        player.cardsName, 
                        player.aggroIncrement, 
                        player.manaMax);
                    LOG.Print("[SERVER] Player created ? {0}", (actor != null) ? "true" : "false");

                    if (battle.Add(actor))
                    {
                        server.Broadcast(player, this).Wait();
                    }
                    break;

                case PD_BattleStart battleStart:
                    if (battle.Init())
                    {
                        server.Broadcast(battleStart, this).Wait();
                    }
                    break;

                case PD_CardPlayed cardPlayed:
                    Card card = actor.hand.Get(cardPlayed.cardIndex);
                    Character character = battle.Actors.Find(a => a.character.Name == cardPlayed.targetName).character;

                    LOG.Print("[SERVER] Found right card ? {0}", (card.name == cardPlayed.cardName) ? "true" : "false");
                    LOG.Print("[SERVER] Found right character ? {0}", (character != null) ? "true" : "false");

                    if (actor.PlayCard(card, character))
                    {
                        server.Broadcast(cardPlayed, this).Wait();
                    }
                    break;

                case PD_TurnEnd turnEnd:
                    if (battle.GetCurrentActor() == actor)
                    {
                        actor.EndTurn();
                        battle.NextActor().StartTurn();
                        server.Broadcast(turnEnd, this).Wait();
                    }
                    break;
            }
        }
    }
}
