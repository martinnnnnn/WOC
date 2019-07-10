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

                case PD_CardPlayed card:
                    Card c = actor.hand.Get(card.cardIndex);
                    Character t = battle.Actors.Find(a => a.character.Name == card.targetName).character;

                    LOG.Print("[SERVER] Found right card ? {0}", (c.name == card.cardName) ? "true" : "false");
                    LOG.Print("[SERVER] Found right character ? {0}", (t != null) ? "true" : "false");

                    if (actor.PlayCard(c, t))
                    {
                        server.Broadcast(card, this).Wait();
                    }
                    break;
            }
        }
    }
}
