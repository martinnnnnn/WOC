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
                new CardEffectDamage(1),
            }));
            cardsMap.Add("card2", new Card("card2", 2, false, new List<CardEffect>
            {
                new CardEffectHeal(2),
                new CardEffectDamage(2),
            }));
            cardsMap.Add("card3", new Card("card3", 3, false, new List<CardEffect>
            {
                new CardEffectHeal(3),
                new CardEffectDamage(3),
            }));


            string jcards = WOC_Core.Serialization.ToJson(cardsMap, true);
            Console.WriteLine(jcards);
            Dictionary<string, Card> cardsMap2 = WOC_Core.Serialization.FromJson<Dictionary<string, Card>>(jcards);

            /*
             * PLAYERS
             */
            Actor a1 = new PlayerActor(battle, new List<string> { "card1", "card1",}, 1, 1, 1, 1);


            /*
             * BATTLE
             */

            cardsMap.ToList().ForEach(pair => battle.Add(pair.Value));
        }
    }
}
