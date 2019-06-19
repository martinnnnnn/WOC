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
        public Dictionary<string, Card> cardsList;
        public List<Actor> Actors;
        public bool isInitialized = false;

        public void FromJson(string data)
        {
            JObject jbattle = (JObject)JToken.Parse(data);

            // initializing cards list
            cardsList = new Dictionary<string, Card>();
            foreach (string jcard in (JArray)jbattle["cards"])
            {
                Card newCard = Card.FromJson(jcard);
                cardsList.Add(newCard.title, newCard);
            }

            // initializing actors
            Actors = new List<Actor>();
            JArray jactors = (JArray)jbattle["actors"];

            Actors = jactors.Select(a => new Actor
            (
                this,
                a["cardsNames"].Values<string>().ToList(),
                (int)a["aggroIncrement"],
                (int)a["manaMax"],
                (int)a["maxInitiative"],
                (int)a["life"]
            )).ToList();
            
            foreach (Actor actor in Actors)
            {
                actor.BattleInit();
            }

            Actors.Sort((a1, a2) => a1.initiative.Value.CompareTo(a2.initiative.Value));

            isInitialized = true;
        }

        public Card GetCard(string cardName)
        {
            return new Card(cardsList[cardName]);
        }

        public void Update()
        {

        }

        public void Shutdown()
        {

        }
    }
}
