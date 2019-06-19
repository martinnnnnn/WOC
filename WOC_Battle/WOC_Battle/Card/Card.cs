using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Battle
{
    public struct PlayInfo
    {
        public Actor Owner;
        public Character Target;

        public void Deconstruct(out Actor owner, out Character target) => (owner, target) = (Owner, Target);
    }

    public class Card
    {
        public List<CardEffect> effects = new List<CardEffect>();
        public string title = "";
        public int manaCost = 0;
        public bool exhaust = false;

        public Card(Card template)
        {
            title = template.title;
            manaCost = template.manaCost;
            exhaust = template.exhaust;
            effects = new List<CardEffect>(template.effects);
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


        static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static string ToJson(Card packet, bool indent = false)
        {
            return JsonConvert.SerializeObject(packet, settings);
        }

        public static Card FromJson(string jcard)
        {
            Card data;
            try
            {
                data = JsonConvert.DeserializeObject<Card>(jpackage, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                data = null;
            }
            return data;
        }
    }
}