using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WOC_Battle;

namespace BattlePlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            Battle battle = new Battle();

            /*
             * CARDS
             */
            Dictionary<string, Card> cardsMap = new Dictionary<string, Card>();
            cardsMap.Add("card1", new Card("card1", 1, false, new List<CardEffect>
            {
                new CardEffectHeal(1),
                new CardEffectDamage(1)
            }));
            cardsMap.Add("card2", new Card("card2", 2, false, new List<CardEffect>
            {
                new CardEffectHeal(2),
                new CardEffectDamage(2)
            }));
            cardsMap.Add("card3", new Card("card3", 3, false, new List<CardEffect>
            {
                new CardEffectHeal(3),
                new CardEffectDamage(3)
            }));
            cardsMap.ToList().ForEach(pair => battle.Add(pair.Value));

            /*
             * PLAYERS && PNJS
             */
            List<Actor> actors = new List<Actor>()
            {
                new PlayerActor(battle, new Character("grrr", 12, 12), "player1", new List<string> { "card1", "card1", }, 2, 30, 4),
                new PlayerActor(battle, new Character("gromelo", 12, 12), "player2", new List<string> { "card2", "card3", }, 3, 30, 6),
                new PlayerActor(battle, new Character("branigan", 12, 12), "player3", new List<string> { "card1", "card3", }, 4, 30, 1),
                new PNJActor(battle, new Character("bouboubou1", 50, 50), "monstre1", 5, 5),
                new PNJActor(battle, new Character("bouboubou2", 50, 50), "monstre2",5, 5),
                new PNJActor(battle, new Character("bouboubou3", 50, 50), "monstre3", 5, 5)
            };

            actors.ForEach(a => battle.Add(a));

            /*
             * BATTLE
             */
            battle.Init();
            battle.OnBattleEnd += BattleOver;

            Console.WriteLine("> Battle starting !");
            while (!isOver)
            {
                Actor current = battle.NextActor();
                switch(current)
                {
                    case PlayerActor player:
                        PlayerTurn(player, battle);
                        break;
                    case PNJActor pnj:
                        Console.WriteLine("> {0} playing !", pnj.Name);
                        break;
                }


            }
        }

        static void PlayerTurn(PlayerActor player, Battle battle)
        {
            player.StartTurn();
            player.DrawCards(2);
            Card card;
            int index = -1;
            do
            {
                Console.Write("> {0}'s turn. Hand : {1}\nWhat's your move (-1 to end turn)? ", player.Name, string.Join(",", player.hand.AsArray().Select(c => c.name).ToArray()));
                index = int.Parse(Console.ReadLine());
                card = player.hand.Remove(index);
                if (card == null && index != -1)
                {
                    Console.WriteLine("You need to give me a valid index !");
                }
            } while (card == null && index != -1);

            bool cardPlayed = false;
            while (!cardPlayed && index != -1)
            {
                Console.Write("> Potential targets : {0}\nWho is your target ?", string.Join(",", battle.Actors.Select(a => a.character.Name).ToArray()));
                index = int.Parse(Console.ReadLine());
                if ((index < 0 || index >= battle.Actors.Count) && index != -1)
                {
                    Console.WriteLine("You need to give me a valid index !");
                }
                else if (index != -1)
                {
                    cardPlayed = player.PlayCard(card, battle.Actors[index].character);
                }
            } 

            Console.WriteLine("> Ending turn !");
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