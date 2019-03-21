using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Network
{
    public class Card
    {
        public string name;
    }

    public class CardIndex
    {
        public Dictionary<string, Card> cards = new Dictionary<string, Card>();
    }

    public class Deck
    {
        string name;
        public List<Card> cards = new List<Card>();

        public static Deck FromJson(string jdeck)
        {
            return JsonConvert.DeserializeObject<Deck>(jdeck);
        }
    }
    public class Character
    {
        public string name;
        public string type;

        public static Character FromJson(string jcharacter)
        {
            return JsonConvert.DeserializeObject<Character>(jcharacter);
        }
    }

    public class Account
    {
        public string name;
        public List<Character> characters = new List<Character>();
        public List<Deck> decks = new List<Deck>();
    }
}
