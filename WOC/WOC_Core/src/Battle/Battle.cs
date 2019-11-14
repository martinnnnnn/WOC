using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;



namespace WOC_Core
{
    namespace RTTS
    {
        public class CardPile
        {
            public Queue<Card> cards = new Queue<Card>();
            public int Count => cards.Count;
            BattlePlayer owner;

            public CardPile(BattlePlayer owner)
            {
                this.owner = owner;
            }

            public Card[] Pop(int count)
            {
                count = Math.Min(count, cards.Count);

                Card[] newCards = new Card[count];

                for (int i = 0; i < count; ++i)
                {
                    newCards[i] = cards.Dequeue();
                }
                return newCards;
            }

            public void Shuffle()
            {
                Card[] tmp = cards.ToArray();
                cards.Clear();

                int n = tmp.Length;
                while (n > 1)
                {
                    int k = owner.battle.random.Next(0, n);
                    n--;
                    Card value = tmp[k];
                    tmp[k] = tmp[n];
                    tmp[n] = value;
                }

                Array.ForEach(tmp, card => cards.Enqueue(card));
            }

            public void Push(Card newCard)
            {
                cards.Enqueue(newCard);
            }

            public void Push(Card[] newCards)
            {
                Array.ForEach(newCards, card => cards.Enqueue(card));
            }

            public Card[] Flush()
            {
                Card[] result = cards.ToArray();
                cards.Clear();
                return result;
            }
        }
        public class Card
        {
            public string name;
            public int timeCost;
            public List<CardEffect> effects = new List<CardEffect>();
        }

        public class Deck
        {
            public string name;
            public List<Card> cards = new List<Card>();
        }

        public class Hand
        {
            List<Card> cards = new List<Card>();
            public List<Card> Cards
            {
                get { return cards;  }
            }

            BattlePlayer owner;

            public Hand(BattlePlayer owner)
            {
                this.owner = owner;
            }

            public int Count => cards.Count;

            public void Add(Card newCard)
            {
                cards.Add(newCard);
            }

            public void Add(Card[] newCards)
            {
                Debug.Assert(newCards != null);
                cards.AddRange(newCards);
            }

            public bool Contains(Card card)
            {
                return cards.Find(c => c.name == card.name) != null;
            }

            public void Remove(int index)
            {
                if (index >= 0 && index < Count)
                {
                    cards.RemoveAt(index);
                }
            }

            public Card Get(int index)
            {
                if (index >= 0 && index < Count)
                {
                    return cards[index];
                }
                return null;
            }

            public Card[] Flush()
            {
                Card[] result = cards.ToArray();
                cards.Clear();
                return result;
            }
        }

        public interface ICombatant { }

        public class BattlePlayer : ICombatant
        {
            // objets
            public Deck deck;
            public Hand hand;
            public CardPile drawPile;
            public CardPile discardPile;
            public Battle battle;
            // battle utils
            public float timeRemaining;
            // actions callbacks
            public Action<float, List<Card>> TurnStarted;

            public BattlePlayer(Battle battle, Deck deck)
            {
                this.battle = battle;
                this.deck = deck;
                hand = new Hand(this);
                drawPile = new CardPile(this);
                discardPile = new CardPile(this);

                drawPile.Push(deck.cards.ToArray());
                drawPile.Shuffle();
            }

            internal void InitTurn(float newTimeRemaining)
            {
                timeRemaining = newTimeRemaining;
                DrawCards(5);
                TurnStarted?.Invoke(timeRemaining, hand.Cards);
            }

            public bool PlayCard(int index)
            {
                Card card = hand.Get(index);

                if (card.timeCost >= timeRemaining)
                {
                    timeRemaining -= card.timeCost;
                    hand.Remove(index);
                    discardPile.Push(card);
                    return true;
                }

                return false;
            }

            public void MoveDiscardToDraw()
            {
                drawPile.Push(discardPile.Flush());
                drawPile.Shuffle();
            }

            public void DrawCards(int count)
            {
                for (int i = 0; i < count; ++i)
                {
                    if (drawPile.Count == 0)
                    {
                        MoveDiscardToDraw();
                    }

                    hand.Add(drawPile.Pop(1));
                }
            }
        }

        public class Monster : ICombatant
        {
            public string name;
        }

        public class Battle
        {
            List<ICombatant> combatants = new List<ICombatant>();
            public float timeRemaining = 60;
            public Action MonsterTurnStarted;

            public int randomSeed = 0;
            public Random random;

            public Battle(List<BattlePlayer> players, List<Monster> monsters, int randomSeed)
            {
                combatants.AddRange(players);
                combatants.AddRange(monsters);
                this.randomSeed = randomSeed;
                random = new Random(this.randomSeed);
            }

            public void MonstersTurnStart()
            {
                MonsterTurnStarted?.Invoke();
                PlayersTurnStart();
            }

            List<BattlePlayer> playingPlayers = new List<BattlePlayer>();
            public void PlayersTurnStart()
            {
                foreach (var player in combatants.Where(c => c is BattlePlayer) as IEnumerable<BattlePlayer>)
                {
                    player.InitTurn(timeRemaining);
                    playingPlayers.Add(player);
                }
            }

            public void PlayerTurnEnd(BattlePlayer player)
            {
                playingPlayers.Remove(player);
                if (playingPlayers.Count == 0)
                {
                    MonstersTurnStart();
                }
            }
        }
    }



















































    public class Battle
    {
        public int RandomSeed = 0;
        public Random random;

        List<Actor> Cemetery = new List<Actor>();
        public List<Actor> Actors = new List<Actor>();
        Actor current;

        public Action OnBattleEnd;
        public bool HasStarted = false;

        public Battle(int randomSeed)
        {
            RandomSeed = randomSeed;
            random = new Random(RandomSeed);
        }

        public bool Start()
        {
            if (!HasStarted)
            {
                Console.WriteLine("[BATTLE] Starting !");
                foreach (Actor actor in Actors)
                {
                    actor.BattleInit();
                }

                UpdateInitiatives();
                NextActor().StartTurn();
                HasStarted = true;
                return true;
            }
            Console.WriteLine("[BATTLE] Already started !");
            return false;
        }

        public void UpdateInitiatives()
        {
            Actors.ForEach(a => a.UpdateInitiative());
            Actors.Sort((a1, a2) => a1.initiative.Value.CompareTo(a2.initiative.Value));
        }

        public Actor NextActor()
        {
            if (current == null)
            {
                current = Actors.First();
            }
            else
            {
                int currentIndex = Actors.FindIndex(a => a.Name == current.Name);
                current = Actors[(currentIndex + 1) % Actors.Count];
            }

            return current;
        }

        public void OnCharacterDeath(Character character)
        {
            Console.WriteLine("{0} died !", character.Name);
            int index = Actors.FindIndex(a => a.Name == character.Owner.Name);
            Actor value = Actors[index];
            Actors.RemoveAt(index);
            Cemetery.Add(value);
            if (Actors.Find(a => a is PNJActor) == null || Actors.Find(a => a is PlayerActor) == null)
            {
                OnBattleEnd();
            }
        }

        public bool Add(Actor actor)
        {
            if (Actors.Find(a => a.Name == actor.Name) == null)
            {
                Actors.Add(actor);
                actor.Battle = this;
                Console.WriteLine("[BATTLE] Actor {0} added.", actor.Name);
                return true;
            }
            Console.WriteLine("[BATTLE] Actor {0} already exists.", actor.Name);
            return false;
        }

        public void Update()
        {

        }

        public void Shutdown()
        {

        }

        public Actor GetCurrentActor()
        {
            return Actors.Find(a => a.turnState == Actor.TurnState.MINE);
        }
    }
}
