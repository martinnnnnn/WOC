using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Core
{
    public struct PlayInfo
    {
        public Actor Owner;
        public Character Target;

        public void Deconstruct(out Actor owner, out Character target) => (owner, target) = (Owner, Target);
    }

    public class Card
    {
        static Dictionary<string, Card> list = new Dictionary<string, Card>();

        public static int Count()
        {
            return list.Count;
        }

        public static Card Get(string cardName)
        {
            return new Card(list[cardName]);
        }

        public static bool Add(Card card)
        {
            if (list.ContainsKey(card.name))
            {
                return false;
            }
            list.Add(card.name, card);
            return true;
        }

        public static void Clear()
        {
            list.Clear();
        }


        public List<CardEffect> effects = new List<CardEffect>();
        public string name = "";
        public int manaCost = 0;
        public bool exhaust = false;
    
        //public Card()
        //{

        //}

        public Card(Card template)
        {
            name = template.name;
            manaCost = template.manaCost;
            exhaust = template.exhaust;
            effects = new List<CardEffect>(template.effects);
            effects.ForEach(e => e.owner = this);
        }

        [JsonConstructor]
        public Card(string name, int manaCost, bool exhaust, List<CardEffect> effects)
        {
            this.name = name;
            this.manaCost = manaCost;
            this.exhaust = exhaust;
            this.effects = effects;
            effects.ForEach(e => e.owner = this);
        }

        public bool Play(PlayInfo info)
        {
            bool result = false;
            foreach (var effect in effects)
            {
                if (effect.Play(info))
                {
                    result = true;
                }
            }
            return result;
        }

        public static void Init(List<Card> cards)
        {
            if (list.Count == 0)
            {
                cards.ForEach(c => Add(c));
            }
        }
    }
}