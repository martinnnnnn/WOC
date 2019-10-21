using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOC_Core;

namespace Playground
{
    public class ClientSession : Session
    {
        //public PlayerActor currentActor;
        public List<PlayerActor> actors = new List<PlayerActor>();
        public Room room;
        //public Battle battle;

        public override void HandleIncomingMessage(IPacketData data)
        {
            base.HandleIncomingMessage(data);

            switch (data)
            {
                case PD_NameModify nameModify:
                    LOG.Print("[SERVER] {0} changed his name to {1}", nameModify.oldName, nameModify.newName);
                    break;

                case PD_Chat chat:
                    LOG.Print(chat.senderName + " : " + chat.message);
                    break;

                case PD_BattlePlayerAdd player:
                    HandlePlayerAdd(player);
                    break;

                case PD_RoomList roomList:
                    LOG.Print("[CLIENT] {0} rooms : {1}", roomList.rooms.Count, string.Join(", ", roomList.rooms));
                    break;

                case PD_PlayerList playerList:
                    LOG.Print("[CLIENT] {0} players in {1} : {2}", playerList.players.Count, string.IsNullOrEmpty(playerList.roomName) ? "lobby" : playerList.roomName, string.Join(", ", playerList.players));
                    break;

                case PD_Validation validation:
                    if (!validation.isValid) LOG.Print("[SERVER] {0}", validation.errorMessage);
                    break;

                case PD_RoomLeave roomLeave:
                    LOG.Print("[SERVER] {0} connected.", roomLeave);
                    break;

                case PD_SessionConnect sessionConnect:
                    LOG.Print("[SERVER] {0} connected.", sessionConnect.name);
                    break;

                case PD_SessionDisconnect sessionDisconnect:
                    LOG.Print("[SERVER] {0} disconnected.", sessionDisconnect.name);
                    break;
            }
        }

        void HandlePlayerAdd(PD_BattlePlayerAdd player)
        {
            PlayerActor newActor = new PlayerActor(
                        new Character(player.charaRace, player.charaCategory, player.charaLife, player.charaName),
                        player.handStartCount,
                        player.name,
                        player.aggroIncrement,
                        player.manaMax);
            actors.Add(newActor);
            newActor.AddCards(player.cardsName);
        }


        



        //public void AddActor_1()
        //{
        //   actor = new PlayerActor(
        //        new Character(Character.Race.ELFE, Character.Category.DRUID, 12, "grrr"),
        //        2,
        //        Name,
        //        2,
        //        30);

        //    battle?.Add(actor);
        //    actor.AddCards(new List<string> { "smol_dmg", "smol_dmg", "smol_dmg", "smol_dmg" });

        //    SendAsync(new PD_PlayerAdd
        //    {
        //        name = Name,
        //        charaRace = Character.Race.ELFE,
        //        charaCategory = Character.Category.DRUID,
        //        charaLife = 12,
        //        charaName = "grrr",
        //        handStartCount = 2,
        //        cardsName = new List<string> { "smol_dmg", "smol_dmg", "smol_dmg", "smol_dmg" },
        //        aggroIncrement = 2,
        //        manaMax = 30
        //    }).Wait();

        //    LOG.Print("[CLIENT] New actor created : {0}", actor.Name);
        //}

        //public void AddActor_2()
        //{
        //    actor = new PlayerActor(
        //        new Character(Character.Race.HUMAN, Character.Category.PALADIN, 12, "gromelo"),
        //        2,
        //        Name, 
        //        3, 
        //        30);
        //    battle?.Add(actor);
        //    actor.AddCards(new List<string> { "hek", "hek", "big_dmg", "big_dmg", "hek", "hek" });

        //    SendAsync(new PD_PlayerAdd
        //    {
        //        name = Name,
        //        charaRace = Character.Race.HUMAN,
        //        charaCategory = Character.Category.PALADIN,
        //        charaLife = 12,
        //        charaName = "gromelo",
        //        handStartCount = 2,
        //        cardsName = new List<string> { "hek", "hek", "big_dmg", "big_dmg", "hek", "hek" },
        //        aggroIncrement = 3,
        //        manaMax = 30
        //    }).Wait();

        //    LOG.Print("[CLIENT] New actor created : {0}", actor.Name);
        //}

        //public void AddActor_3()
        //{
        //    actor = new PlayerActor(
        //        new Character(Character.Race.ELFE, Character.Category.SORCERER, 12, "branigan"),
        //        2,
        //        Name,
        //        4,
        //        30);
        //    battle?.Add(actor);
        //    actor.AddCards(new List<string> { "smol_dmg", "smol_dmg", "big_dmg", "big_dmg", "big_dmg", "big_dmg", "hek" });

        //    SendAsync(new PD_PlayerAdd
        //    {
        //        name = Name,
        //        charaRace = Character.Race.ELFE,
        //        charaCategory = Character.Category.SORCERER,
        //        charaLife = 12,
        //        charaName = "branigan",
        //        handStartCount = 2,
        //        cardsName = new List<string> { "smol_dmg", "smol_dmg", "big_dmg", "big_dmg", "big_dmg", "big_dmg", "hek" },
        //        aggroIncrement = 4,
        //        manaMax = 30
        //    }).Wait();

        //    LOG.Print("[CLIENT] New actor created : {0}", actor.Name);
        //}
    }
}
