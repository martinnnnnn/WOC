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

    public class Character
    {
        public string name;
        public string type;
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

    public class Monster
    {
        public string name;
    }

    public class Player
    {
        public int life;
        public int mana;
        public int aggro;
        public int initiative;
        public List<Card> deck = new List<Card>();
        public List<Card> drawPile = new List<Card>();
        public List<Card> discardPile = new List<Card>();
        public List<Card> hand = new List<Card>();
    }

    public class BattleInfo
    {
        public string name;
        public List<string> accountNames = new List<string>();
        public Monster monster;
    }


}

/*


    Monster
        string name

    Battle:
        string name
        Monster monster
        List SessionInfo


 */

