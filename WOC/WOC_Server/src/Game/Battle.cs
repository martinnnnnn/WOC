using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using WOC_Core;

//namespace WOC_Server
//{
//    public class CardPile
//    {
//        public Queue<Card> cards = new Queue<Card>();
//        public int Count => cards.Count;
//        BattlePlayer owner;

//        public CardPile(BattlePlayer owner)
//        {
//            this.owner = owner;
//        }

//        public Card[] Pop(int count)
//        {
//            count = Math.Min(count, cards.Count);

//            Card[] newCards = new Card[count];

//            for (int i = 0; i < count; ++i)
//            {
//                newCards[i] = cards.Dequeue();
//            }
//            return newCards;
//        }

//        public Card Pop()
//        {
//            return cards.Dequeue();
//        }

//        public void Shuffle()
//        {
//            Card[] tmp = cards.ToArray();
//            cards.Clear();

//            int n = tmp.Length;
//            while (n > 1)
//            {
//                int k = owner.battle.random.Next(0, n);
//                n--;
//                Card value = tmp[k];
//                tmp[k] = tmp[n];
//                tmp[n] = value;
//            }

//            Array.ForEach(tmp, card => cards.Enqueue(card));
//        }

//        public void Push(Card newCard)
//        {
//            cards.Enqueue(newCard);
//        }

//        public void Push(Card[] newCards)
//        {
//            Array.ForEach(newCards, card => cards.Enqueue(card));
//        }

//        public Card[] Flush()
//        {
//            Card[] result = cards.ToArray();
//            cards.Clear();
//            return result;
//        }
//    }
//    public class Card
//    {
//        public string name;
//        public int timeCost;
//        public List<CardEffect> effects = new List<CardEffect>();
//    }

//    public class Deck
//    {
//        public string name;
//        public List<Card> cards = new List<Card>();
//    }

//    public class Hand
//    {
//        List<Card> cards = new List<Card>();
//        public List<Card> Cards
//        {
//            get { return cards; }
//        }

//        BattlePlayer owner;

//        public Hand(BattlePlayer owner)
//        {
//            this.owner = owner;
//        }

//        public int Count => cards.Count;

//        public void Add(Card newCard)
//        {
//            cards.Add(newCard);
//        }

//        public void Add(Card[] newCards)
//        {
//            Debug.Assert(newCards != null);
//            cards.AddRange(newCards);
//        }

//        public bool Contains(Card card)
//        {
//            return cards.Find(c => c.name == card.name) != null;
//        }

//        public void Remove(int index)
//        {
//            if (index >= 0 && index < Count)
//            {
//                cards.RemoveAt(index);
//            }
//        }

//        public Card Get(int index)
//        {
//            if (index >= 0 && index < Count)
//            {
//                return cards[index];
//            }
//            return null;
//        }

//        public Card[] Flush()
//        {
//            Card[] result = cards.ToArray();
//            cards.Clear();
//            return result;
//        }
//    }

//    public interface ICombatant { }

//    public class BattlePlayer : ICombatant
//    {
//        public string name;
//        public int location;
//        // objets
//        public Deck deck;
//        public Hand hand;
//        public CardPile drawPile;
//        public CardPile discardPile;
//        public Battle battle;
//        // battle utils
//        public float timeRemaining;
//        // actions callbacks
//        public Action<string, Card> CardDrawn;

//        public BattlePlayer(string name, int location, Deck deck)
//        {
//            this.name = name;
//            this.location = location;
//            this.deck = deck;
//            hand = new Hand(this);
//            drawPile = new CardPile(this);
//            discardPile = new CardPile(this);

//            drawPile.Push(deck.cards.ToArray());
//        }

//        public void Init(Battle battle)
//        {
//            this.battle = battle;
//            drawPile.Shuffle();
//        }

//        public void Update(float dt)
//        {
//            timeRemaining -= dt;
//            if (timeRemaining <= 0)
//            {
//                battle.PlayerTurnEnd(this);
//            }
//        }

//        public void InitTurn(float newTimeRemaining)
//        {
//            timeRemaining = newTimeRemaining;
//            DrawCards(5);
//        }

//        public bool PlayCard(int index, string targetName, bool force = false)
//        {
//            Card card = hand.Get(index);

//            if (card.timeCost >= timeRemaining || force)
//            {
//                timeRemaining -= card.timeCost;
//                hand.Remove(index);
//                discardPile.Push(card);
//                return true;
//            }

//            return false;
//        }

//        public bool PlayCard(string cardName, string targetName, bool force = false)
//        {
//            Card card = hand.Cards.Find(c => c.name == cardName);
//            if (card != null)
//            {
//                int index = hand.Cards.IndexOf(card);
//                return PlayCard(index, targetName, force);
//            }
//            return false;
//        }

//        public void MoveDiscardToDraw()
//        {
//            drawPile.Push(discardPile.Flush());
//            drawPile.Shuffle();
//        }

//        public void DrawCards(int count)
//        {
//            for (int i = 0; i < count; ++i)
//            {
//                if (drawPile.Count == 0)
//                {
//                    MoveDiscardToDraw();
//                }

//                Card newCard = drawPile.Pop();
//                hand.Add(newCard);
//                CardDrawn?.Invoke(name, newCard);
//            }
//        }
//    }

//    public class Monster : ICombatant
//    {
//        public string name;
//        public float baseTime;

//        public Monster(string name, float baseTime)
//        {
//            this.name = name;
//            this.baseTime = baseTime;
//        }
//    }

//    public class Battle
//    {
//        public List<BattlePlayer> players = new List<BattlePlayer>();
//        public List<Monster> monsters = new List<Monster>();

//        public float timeRemaining = 60;
//        public Action MonsterTurnStarted;

//        public Random random;
//        public bool hasStarted = false;
//        public Battle(List<BattlePlayer> players, List<Monster> monsters)
//        {
//            this.players.AddRange(players);
//            this.monsters.AddRange(monsters);
//            random = new Random();
//            hasStarted = true;
//            this.players.ForEach(p =>
//            {
//                p.Init(this);
//            });

//        }

//        public void Update(float dt)
//        {
//            playingPlayers.ForEach(p => p.Update(dt));
//        }

//        public void MonstersTurnStart()
//        {
//            MonsterTurnStarted?.Invoke();
//            PlayersTurnStart();
//        }

//        List<BattlePlayer> playingPlayers = new List<BattlePlayer>();
//        public void PlayersTurnStart()
//        {
//            foreach (var player in players)
//            {
//                player.InitTurn(monsters[0].baseTime);
//                playingPlayers.Add(player);
//            }
//        }
        
//        public void PlayerTurnEnd(BattlePlayer player)
//        {
//            playingPlayers.Remove(player);
//            if (playingPlayers.Count == 0)
//            {
//                MonstersTurnStart();
//            }
//        }

//        public void PlayerTurnEnd(string playerName)
//        {
//            PlayerTurnEnd(playingPlayers.Find(p => p.name == playerName));
//        }

//        public bool PlayCard(string playerName, int cardIndex, string targetName, bool force = false)
//        {
//            var player = playingPlayers.Find(p => p.name == playerName);
//            if (player != null)
//            {
//                return player.PlayCard(cardIndex, targetName, force);
//            }

//            return false;
//        }
//    }
//}
