using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WOC_Core;

namespace BattlePlayground
{
    class Program
    {
        static Battle battle;

        static void Main(string[] args)
        {
            battle = new Battle();

            // INIT
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
            cardsMap.ForEach(c => battle.Add(c));

            //PLAYERS && PNJS
            List<Actor> actors = new List<Actor>()
            {
                // battle | character(race, category, life, name) | hand | name | deck | aggroIncrement | max mana
                new PlayerActor(battle, new Character(Character.Race.ELFE, Character.Category.DRUID, 12, "grrr"), new Hand(2, 3), "player1", new List<string> { "smol_dmg", "smol_dmg", "smol_dmg", "smol_dmg" }, 2, 30),
                new PlayerActor(battle, new Character(Character.Race.HUMAN, Character.Category.PALADIN, 12, "gromelo"), new Hand(2, 3), "player2", new List<string> { "hek", "hek", "big_dmg", "big_dmg", "hek" }, 3, 30),
                new PlayerActor(battle, new Character(Character.Race.ELFE, Character.Category.SORCERER, 12, "branigan"), new Hand(2, 3), "player3", new List<string> { "smol_dmg", "smol_dmg", "big_dmg", "big_dmg", "big_dmg", "big_dmg" }, 4, 30),
                // battle | character | name | first init
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 50), "monstre1", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 50), "monstre2", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.CHAMAN, 50), "monstre3", 5)
            };
            actors.ForEach(a => battle.Add(a));

            RunBattle();
        }

        static void RunBattle()
        {
            battle.Start();
            battle.OnBattleEnd += BattleOver;

            LOG.Print("> Battle starting !");
            while (!isOver)
            {
                Actor current = battle.NextActor();
                switch (current)
                {
                    case PlayerActor player:
                        PlayerTurn(player, battle);
                        break;
                    case PNJActor pnj:
                        LOG.Print("> {0} playing !", pnj.Name);
                        break;
                }
            }
        }

        static void PlayerTurn(PlayerActor player, Battle battle)
        {
            player.StartTurn();
            Card card;
            int cardIndex = -1;
            do
            {
                Console.Write("> {0}'s turn. Hand : {1}\n> What's your move (-1 to end turn)? ", player.Name, string.Join(",", player.hand.AsArray().Select(c => c.name).ToArray()));
                cardIndex = int.Parse(Console.ReadLine());
                card = player.hand.Get(cardIndex);
                if (card == null && cardIndex != -1)
                {
                    LOG.Print("You need to give me a valid index !");
                }
            } while (card == null && cardIndex != -1);

            bool cardPlayed = false;
            int targetIndex = -1;
            while (!cardPlayed && targetIndex != -1)
            {
                Console.Write("> Potential targets : {0}\nWho is your target ?", string.Join(",", battle.Actors.Select(a => a.character.Name).ToArray()));
                targetIndex = int.Parse(Console.ReadLine());
                if ((targetIndex < 0 || targetIndex >= battle.Actors.Count) && targetIndex != -1)
                {
                    LOG.Print("You need to give me a valid index !");
                }
                else if (targetIndex != -1)
                {
                    cardPlayed = player.PlayCard(card, battle.Actors[targetIndex].character);
                }
            }

            LOG.Print("> Ending turn !");
            player.EndTurn();
        }


        static bool isOver = false;
        static void BattleOver()
        {
            isOver = true;
        }
    }
}
//Card[] cardsfound = Array.FindAll(player.hand.AsArray(), c => c.name == cardName);
//if (cardsfound.Length > 0)
//{
//    cardPlayed = cardsfound[];
//}