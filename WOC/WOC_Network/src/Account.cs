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
    public class Deck
    {
        public string name;
        public List<Card> cards = new List<Card>();
    }

    public class Power
    {
        public string name;
        public string effect;
    }

    public class Character
    {
        public string name;
        public string type;
        public Power power;
    }

    public class Account
    {
        public string name;
        public string password;
        public List<Character> characters = new List<Character>();
        public List<Deck> decks = new List<Deck>();
    }
    public class AccountList
    {
        public List<string> names;
    }

    public class BattleInfo
    {
        public string name;
        public List<string> accountNames = new List<string>();
    }
}
