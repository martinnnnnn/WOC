using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json.Linq;



namespace WOC_Battle
{
    public class Battle
    {
        Dictionary<string, Card> cardsMap;
        List<Actor> Actors;
        public void FromJson(string data)
        {
            JObject jbattle = (JObject)JToken.Parse(data);

            // initializing cards list
            cardsMap = new Dictionary<string, Card>();
            foreach (string jcard in (JArray)jbattle["cards"])
            {
                Card newCard = WOC_Core.Serialization.FromJson<Card>(jcard);
                cardsMap.Add(newCard.name, newCard);
            }

            // initializing actors
            Actors = new List<Actor>();
            JArray jactors = (JArray)jbattle["actors"];

            //Actors = jactors.Select(a => new Actor
            //(
            //    this,
            //    a["cardsNames"].Values<string>().ToList(),
            //    (int)a["aggroIncrement"],
            //    (int)a["manaMax"],
            //    (int)a["maxInitiative"],
            //    (int)a["life"]
            //)).ToList();
            
            foreach (Actor actor in Actors)
            {
                actor.BattleInit();
            }

            Actors.Sort((a1, a2) => a1.initiative.Value.CompareTo(a2.initiative.Value));
        }

        public Card GetCard(string cardName)
        {
            return new Card(cardsMap[cardName]);
        }

        public bool Add(Card card)
        {
            if (cardsMap.ContainsKey(card.name))
            {
                return false;
            }
            cardsMap.Add(card.name, card);
            return true;
        }

        public void Update()
        {

        }

        public void Shutdown()
        {

        }
    }
}
