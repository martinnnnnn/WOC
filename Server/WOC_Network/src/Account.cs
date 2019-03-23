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
        public List<Character> characters = new List<Character>();
        public List<Deck> decks = new List<Deck>();
    }
}

/*

    Card:
        string name

    CardList:
        Dictionary string, Card

    Deck:
        string name
        list Card

    Character:
        string name
        string prefabPath

    AccountInfo: 
        string name
        list Deck
        list Character

    ConnectionInfo:
        Socket
        Buffer

    SessionInfo:
        AccountInfo
        ConnectionInfo

    Monster
        string name

    Battle:
        string name
        Monster monster
        List SessionInfo


    #### Packets ####
    
    Create account
    Connect account
    List account
    Disconnect account

 */

