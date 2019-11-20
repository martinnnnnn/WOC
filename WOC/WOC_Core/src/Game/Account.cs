using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Core
{
    public class Deck
    {
        public string name;
        public List<Card> cards = new List<Card>();
    }

    public class Account
    {
        public string email;
        public string password;
        public string name;
        public List<string> friends = new List<string>();

        public List<string> partyAccounts = new List<string>();

        public bool connected = false;

        public Deck currentDeck = null;
        public List<Deck> decks = new List<Deck>();

        public Session session = null;
        
        public bool SetCurrentDeck(string name)
        {
            currentDeck = decks.Find(c => c.name == name);
            return currentDeck != null;
        }
    }

    public class Group
    {
        public string leaderName;
        public List<Account> accounts = new List<Account>();
    }
}
