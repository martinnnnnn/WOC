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
        public List<string> cardNames = new List<string>();
    }

    public class Account
    {
        public string email;
        public string password;
        public string name;
        public List<string> friends = new List<string>();


        public bool connected = false;
        public PlayerActor actor = null;

        public Character defaultCharacter = null;
        public List<Character> characters = new List<Character>();

        public Deck defaultDeck = null;
        public List<Deck> decks = new List<Deck>();

        public Session session = null;

        //TODO game history stats

        public void SetDefaultCharacter(string name)
        {
            defaultCharacter = characters.Find(c => c.Name == name);
            Debug.Assert(defaultCharacter != null);
        }

    }
}
