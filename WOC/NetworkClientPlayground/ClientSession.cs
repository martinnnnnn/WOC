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
            LOG.Print("[CLIENT] received a packet. {0}", data);
            base.HandleIncomingMessage(data);

            switch (data)
            {
                case PD_Chat chat:
                    LOG.Print(chat.senderName + " : " + chat.message);
                    break;

                case PD_PlayerAdd player:
                    PlayerActor newActor = new PlayerActor(
                        battle,
                        new Character(player.charaRace, player.charaCategory, player.charaLife, player.charaName),
                        new Hand(player.handStartCount, player.handMaxCount),
                        player.name,
                        player.cardsName,
                        player.aggroIncrement,
                        player.manaMax);
                    battle.Add(newActor);
                    break;

                case PD_CardPlayed cardPlayed:
                    PlayerActor owner = battle.Actors.Find(a => cardPlayed.ownerName == a.Name) as PlayerActor;
                    Card card = owner.hand.Get(cardPlayed.cardIndex);
                    Character target = battle.Actors.Find(a => a.character.Name == cardPlayed.targetName).character;

                    LOG.Print("[CLIENT] Found Actor ? {0}", (cardPlayed.ownerName == actor.Name) ? "true" : "false");
                    LOG.Print("[CLIENT] Found Card ? {0}", (card.name == cardPlayed.cardName) ? "true" : "false");
                    LOG.Print("[CLIENT] Found Character ? {0}", (target != null) ? "true" : "false");
                    LOG.Print("[CLIENT] Card played ? {0}", (owner.PlayCard(card, target)) ? "true" : "false");
                    break;

                case PD_TurnEnd turnEnd:
                    battle.GetCurrentActor().EndTurn();
                    battle.NextActor().StartTurn();
                    //LOG.Print("[CLIENT] New turn starts for {0}.", battle.GetCurrentActor());
                    break;

                case PD_BattleStart battleStart:
                    battle.Init();
                    break;
            }
        }

        public void Init()
        {
            LOG.Print("[CLIENT] Battle initialization...");
            battle = new Battle();

            Initiative.Max = 50;

            // CARDS
            List<Card> cardsMap = new List<Card>()
            {
                // name | mana cost | exhaust | effects list
                new Card("smol_dmg", 1, false, new List<CardEffect>
                {
                    new CardEffectDamage(1)
                }),
                new Card("hek", 2, false, new List<CardEffect>
                {
                    new CardEffectHeal(2)
                }),
                new Card("big_dmg", 3, false, new List<CardEffect>
                {
                    new CardEffectDamage(4)
                })
            };
            LOG.Print("[CLIENT] Adding cards");
            cardsMap.ForEach(c => battle.Add(c));

            //PNJS
            List<Actor> actors = new List<Actor>()
            {
                // battle | character | name | first init
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 50), "monstre1", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 50), "monstre2", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.CHAMAN, 50), "monstre3", 5)
            };
            LOG.Print("[CLIENT] Adding PNJs");
            actors.ForEach(a => battle.Add(a));
        }

        public void RunBattle()
        {
            LOG.Print("[CLIENT] Battle starting !");
            while (!isOver)
            {
                Actor current = battle.NextActor();
                switch (current)
                {
                    case PlayerActor player:
                        player.StartTurn();
                        break;
                    case PNJActor pnj:
                        LOG.Print("[SERVER] {0} playing !", pnj.Name);
                        break;
                }
            }
        }
        
        bool isOver = false;
        void BattleOver()
        {
            isOver = true;
        }

        public void AddActor_1()
        {
            actor = new PlayerActor(
                battle,
                new Character(Character.Race.ELFE, Character.Category.DRUID, 12, "grrr"),
                new Hand(2, 3),
                "player1",
                new List<string> { "smol_dmg", "smol_dmg", "smol_dmg", "smol_dmg" },
                2,
                30);

            battle.Add(actor
            );

            SendAsync(new PD_PlayerAdd
            {
                name = "player1",
                charaRace = Character.Race.ELFE,
                charaCategory = Character.Category.DRUID,
                charaLife = 12,
                charaName = "grrr",
                handStartCount = 2,
                handMaxCount = 3,
                cardsName = new List<string> { "smol_dmg", "smol_dmg", "smol_dmg", "smol_dmg" },
                aggroIncrement = 2,
                manaMax = 30
            }).Wait();

            LOG.Print("[CLIENT] New actor created : {0}", actor.Name);
        }

        public void AddActor_2()
        {
            actor = new PlayerActor(battle, new Character(Character.Race.HUMAN, Character.Category.PALADIN, 12, "gromelo"), new Hand(2, 3), "player2", new List<string> { "hek", "hek", "big_dmg", "big_dmg", "hek" }, 3, 30);
            battle.Add(actor);

            SendAsync(new PD_PlayerAdd
            {
                name = "player2",
                charaRace = Character.Race.HUMAN,
                charaCategory = Character.Category.PALADIN,
                charaLife = 12,
                charaName = "gromelo",
                handStartCount = 2,
                handMaxCount = 3,
                cardsName = new List<string> { "hek", "hek", "big_dmg", "big_dmg", "hek", "hek" },
                aggroIncrement = 3,
                manaMax = 30
            }).Wait();

            LOG.Print("[CLIENT] New actor created : {0}", actor.Name);
        }

        public void AddActor_3()
        {
            actor = new PlayerActor(battle, new Character(Character.Race.ELFE, Character.Category.SORCERER, 12, "branigan"), new Hand(2, 3), "player3", new List<string> { "smol_dmg", "smol_dmg", "big_dmg", "big_dmg", "big_dmg", "big_dmg" }, 4, 30);
            battle.Add(actor);

            SendAsync(new PD_PlayerAdd
            {
                name = "player3",
                charaRace = Character.Race.ELFE,
                charaCategory = Character.Category.SORCERER,
                charaLife = 12,
                charaName = "branigan",
                handStartCount = 2,
                handMaxCount = 3,
                cardsName = new List<string> { "smol_dmg", "smol_dmg", "big_dmg", "big_dmg", "big_dmg", "big_dmg", "hek" },
                aggroIncrement = 4,
                manaMax = 30
            }).Wait();

            LOG.Print("[CLIENT] New actor created : {0}", actor.Name);
        }
    }
}
