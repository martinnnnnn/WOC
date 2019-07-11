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
        public PlayerActor actor;
        public Battle battle;

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

                case PD_PlayerAdd player:
                    HandlePlayerAdd(player);
                    break;

                case PD_CardPlayed cardPlayed:
                    PlayerActor owner = battle.Actors.Find(a => cardPlayed.ownerName == a.Name) as PlayerActor;
                    Card card = owner.hand.Get(cardPlayed.cardIndex);
                    Character target = battle.Actors.Find(a => a.character.Name == cardPlayed.targetName).character;
                    owner.PlayCard(card, target);
                    break;

                case PD_TurnEnd turnEnd:
                    battle.GetCurrentActor().EndTurn();
                    battle.NextActor().StartTurn();
                    if (battle.GetCurrentActor() == actor)
                    {
                        LOG.Print("[PLAYGROUND] It's my turn !");
                    }
                    break;

                case PD_BattleStart battleStart:
                    battle.Start();
                    if (battle.GetCurrentActor() == actor)
                    {
                        LOG.Print("[PLAYGROUND] It's my turn !");
                    }
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

                case PD_RoomJoin roomJoin:
                    if (roomJoin.playerName == Name)
                    {
                        LOG.Print("[CLIENT] Welcome to {0}.", roomJoin.roomName);
                        InitBattle(roomJoin.randomSeed);
                    }
                    else
                    {
                        LOG.Print("[CLIENT] {0} just joined.", roomJoin.playerName);
                        //if (roomJoin.playerInfo != null)
                        //{
                        //    HandlePlayerAdd(roomJoin.playerInfo as PD_PlayerAdd);
                        //}
                    }
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

        void HandlePlayerAdd(PD_PlayerAdd player)
        {
            PlayerActor newActor = new PlayerActor(
                        battle,
                        new Character(player.charaRace, player.charaCategory, player.charaLife, player.charaName),
                        player.handStartCount,
                        player.name,
                        player.cardsName,
                        player.aggroIncrement,
                        player.manaMax);
            battle.Add(newActor);
        }

        public void InitBattle(int randomSeem)
        {
            LOG.Print("[CLIENT] Battle initialization...");
            battle = new Battle(randomSeem);
            battle.OnBattleEnd += BattleOver;

            Initiative.Max = 50;
            Hand.Max = 3;

            // CARDS
            List<Card> cardsMap = new List<Card>()
            {
                // name | mana cost | exhaust | effects list
                new Card("smol_dmg", 1, false, new List<CardEffect>
                {
                    new CardEffectDamage(5)
                }),
                new Card("hek", 2, false, new List<CardEffect>
                {
                    new CardEffectHeal(2)
                }),
                new Card("big_dmg", 3, false, new List<CardEffect>
                {
                    new CardEffectDamage(10)
                })
            };
            LOG.Print("[CLIENT] Adding cards");
            cardsMap.ForEach(c => battle.Add(c));

            //PNJS
            List<Actor> actors = new List<Actor>()
            {
                // battle | character | name | first init
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre1", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre2", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.CHAMAN, 15), "monstre3", 5)
            };
            LOG.Print("[CLIENT] Adding PNJs");
            actors.ForEach(a => battle.Add(a));
        }

        void BattleOver()
        {
            if (actor.character.Life > 0)
            {
                LOG.Print("You won !!! ");
            }
        }

        public void AddActor_1()
        {
            actor = new PlayerActor(
                battle,
                new Character(Character.Race.ELFE, Character.Category.DRUID, 12, "grrr"),
                2,
                Name,
                new List<string> { "smol_dmg", "smol_dmg", "smol_dmg", "smol_dmg" },
                2,
                30);

            battle.Add(actor
            );

            SendAsync(new PD_PlayerAdd
            {
                name = Name,
                charaRace = Character.Race.ELFE,
                charaCategory = Character.Category.DRUID,
                charaLife = 12,
                charaName = "grrr",
                handStartCount = 2,
                cardsName = new List<string> { "smol_dmg", "smol_dmg", "smol_dmg", "smol_dmg" },
                aggroIncrement = 2,
                manaMax = 30
            }).Wait();

            LOG.Print("[CLIENT] New actor created : {0}", actor.Name);
        }

        public void AddActor_2()
        {
            actor = new PlayerActor(
                battle, 
                new Character(Character.Race.HUMAN, Character.Category.PALADIN, 12, "gromelo"),
                2,
                Name, 
                new List<string> { "hek", "hek", "big_dmg", "big_dmg", "hek", "hek" },
                3, 
                30);
            battle.Add(actor);

            SendAsync(new PD_PlayerAdd
            {
                name = Name,
                charaRace = Character.Race.HUMAN,
                charaCategory = Character.Category.PALADIN,
                charaLife = 12,
                charaName = "gromelo",
                handStartCount = 2,
                cardsName = new List<string> { "hek", "hek", "big_dmg", "big_dmg", "hek", "hek" },
                aggroIncrement = 3,
                manaMax = 30
            }).Wait();

            LOG.Print("[CLIENT] New actor created : {0}", actor.Name);
        }

        public void AddActor_3()
        {
            actor = new PlayerActor(
                battle,
                new Character(Character.Race.ELFE, Character.Category.SORCERER, 12, "branigan"),
                2,
                Name,
                new List<string> { "smol_dmg", "smol_dmg", "big_dmg", "big_dmg", "big_dmg", "big_dmg", "hek" },
                4,
                30);
            battle.Add(actor);

            SendAsync(new PD_PlayerAdd
            {
                name = Name,
                charaRace = Character.Race.ELFE,
                charaCategory = Character.Category.SORCERER,
                charaLife = 12,
                charaName = "branigan",
                handStartCount = 2,
                cardsName = new List<string> { "smol_dmg", "smol_dmg", "big_dmg", "big_dmg", "big_dmg", "big_dmg", "hek" },
                aggroIncrement = 4,
                manaMax = 30
            }).Wait();

            LOG.Print("[CLIENT] New actor created : {0}", actor.Name);
        }
    }
}
