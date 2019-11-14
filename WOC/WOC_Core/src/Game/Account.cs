﻿using System;
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

    public struct Vector2
    {
        public float X;
        public float Y;
    }

    public class WorldPlayer
    {
        public Vector2 position;
        public Vector2 velocity;
        public int life;
        public int lifeMax;
    }
        
}
